// Context to manage authentication state and handle token rotation globally
import React, {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useRef,
  useState
} from "react";
import type { AccessTokenDto, UserDto } from "../data/userdtos";
import { fetchCurrentUser, refreshAuthToken, API_BASE_URL } from "../api/api";

interface AuthContextType {
  isLoggedIn: boolean;
  token: string | null;
  currentUser: UserDto | null;
  setCurrentUser: (user: UserDto | null) => void;
  login: (tokens: AccessTokenDto) => void;
  logout: () => void;
  refreshUser: () => Promise<void>;
  ensureAccessToken: () => Promise<string | null>;
  // true after initial silent auth check (refresh attempt) finished
  initialized: boolean;
}
// Keep only access token + expiry in-memory
type AuthTokens = {
  accessToken: string;
  accessTokenExpiresUtc: string;
};



const AuthContext = createContext<AuthContextType>({
  isLoggedIn: false,
  token: null,
  currentUser: null,
  setCurrentUser: () => {},
  login: () => {},
  logout: () => {},
  refreshUser: async () => {},
  ensureAccessToken: async () => null,
  initialized: false,
});

export const useAuth = () => useContext(AuthContext);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  // start with no tokens in memory (no localStorage)
  const [tokens, setTokens] = useState<AuthTokens | null>(null);
  const [currentUser, setCurrentUser] = useState<UserDto | null>(null);
  const [initialized, setInitialized] = useState(false);
  const tokensRef = useRef<AuthTokens | null>(tokens);
  const refreshTimeoutRef = useRef<number | null>(null);

  // Keep tokensRef in sync
  useEffect(() => {
    tokensRef.current = tokens;
  }, [tokens]);

  // In-memory only: do not persist refresh token or access token to localStorage
  const persistTokens = useCallback((next: AuthTokens | null) => {
    setTokens(next);
  }, []);

  // On first load, attempt a silent refresh using the HttpOnly cookie.
  // If the refresh succeeds we populate the in-memory access token so the user stays logged in after a full page reload.
  useEffect(() => {
    let mounted = true;
    (async () => {
      try {
        const updated = await refreshAuthToken(); // sends cookie via credentials: "include"
        if (!mounted || !updated) return;
        persistTokens({
          accessToken: updated.accessToken,
          accessTokenExpiresUtc: updated.accessTokenExpiresUtc,
        });
      } catch {
        // Silent failure: remain logged out (do not call logout() here to avoid clearing cookie aggressively)
      } finally {
        // signal that initial auth check completed (success or failure)
        if (mounted) setInitialized(true);
      }
    })();
    return () => { mounted = false; };
  }, [persistTokens]);

  // ensure login sets initialized so UI can react immediately after user logs in
  const wrappedLogin = useCallback((nextTokens: AccessTokenDto) => {
    persistTokens({
      accessToken: nextTokens.accessToken,
      accessTokenExpiresUtc: nextTokens.accessTokenExpiresUtc,
    });
    setInitialized(true);
  }, [persistTokens]);

  const clearRefreshTimeout = useCallback(() => {
    if (refreshTimeoutRef.current !== null) {
      window.clearTimeout(refreshTimeoutRef.current);
      refreshTimeoutRef.current = null;
    }
  }, []);

  const logout = useCallback(() => {
    clearRefreshTimeout();
    // best-effort: notify backend to clear HttpOnly refresh cookie on the API origin
    try {
      void fetch(`${API_BASE_URL}/logout`, { method: "POST", credentials: "include" });
    } catch {}
    persistTokens(null);
    setCurrentUser(null);
  }, [clearRefreshTimeout, persistTokens]);

  // expose wrappedLogin instead of raw login so initialized flag is set
  const login = wrappedLogin;

  const refreshAccessToken = useCallback(async (): Promise<AuthTokens | null> => {
    try {
      // The backend reads the refresh token from the HttpOnly cookie; call refresh endpoint (no token in JS)
      const updated = await refreshAuthToken();
      // updated must include accessToken and accessTokenExpiresUtc
      persistTokens({
        accessToken: updated.accessToken,
        accessTokenExpiresUtc: updated.accessTokenExpiresUtc,
      });
      return {
        accessToken: updated.accessToken,
        accessTokenExpiresUtc: updated.accessTokenExpiresUtc,
      };
    } catch (error) {
      logout();
      throw error;
    }
  }, [logout, persistTokens]);

  const ensureAccessToken = useCallback(async (): Promise<string | null> => {
    const current = tokensRef.current;
    if (!current) return null;

    const expiresAt = Date.parse(current.accessTokenExpiresUtc);
    const now = Date.now();
    const skewMs = 30_000; // refresh 30 seconds before expiry

    if (Number.isNaN(expiresAt) || expiresAt - skewMs <= now) {
      const refreshed = await refreshAccessToken();
      return refreshed?.accessToken ?? null;
    }

    return current.accessToken;
  }, [refreshAccessToken]);

  const refreshUser = useCallback(async () => {
    const accessToken = await ensureAccessToken();
    if (!accessToken) {
      setCurrentUser(null);
      return;
    }

    try {
      const user = await fetchCurrentUser(accessToken);
      setCurrentUser(user);
    } catch {
      setCurrentUser(null);
    }
  }, [ensureAccessToken]);

  useEffect(() => {
    clearRefreshTimeout();
    if (!tokens) {
      return;
    }

    const expiresAt = Date.parse(tokens.accessTokenExpiresUtc);
    if (Number.isNaN(expiresAt)) {
      return;
    }

    const leadMs = 60_000; // refresh one minute before expiry
    const delay = Math.max(1_000, expiresAt - Date.now() - leadMs);
    const timeoutId = window.setTimeout(() => {
      refreshAccessToken().catch(() => {
        // refreshAccessToken will handle logout on failure
      });
    }, delay);
    refreshTimeoutRef.current = timeoutId;

    return () => {
      clearRefreshTimeout();
    };
  }, [tokens, refreshAccessToken, clearRefreshTimeout]);

  useEffect(() => {
    if (tokens) {
      refreshUser();
    } else {
      setCurrentUser(null);
    }
  }, [tokens, refreshUser]);

  const contextValue = useMemo<AuthContextType>(() => ({
    isLoggedIn: !!tokens,
    token: tokens?.accessToken ?? null,
    currentUser,
    setCurrentUser,
    login,
    logout,
    refreshUser,
    ensureAccessToken,
    initialized,
  }), [tokens, currentUser, login, logout, refreshUser, ensureAccessToken, initialized]);

  return (
    <AuthContext.Provider value={contextValue}>
      {children}
    </AuthContext.Provider>
  );
};