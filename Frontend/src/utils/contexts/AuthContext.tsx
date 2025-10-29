//** Auth context with in-memory tokens and auto-refresh */
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
  // Set after the first silent refresh attempt finishes
  initialized: boolean;
}
// In-memory access token + expiry only
type AuthTokens = {
  accessToken: string;
  accessTokenExpiresUtc: string;
};

const AuthContext = createContext<AuthContextType>({
  isLoggedIn: false,
  token: null,
  currentUser: null,
  setCurrentUser: () => { },
  login: () => { },
  logout: () => { },
  refreshUser: async () => { },
  ensureAccessToken: async () => null,
  initialized: false,
});

export const useAuth = () => useContext(AuthContext);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [tokens, setTokens] = useState<AuthTokens | null>(null);
  const [currentUser, setCurrentUser] = useState<UserDto | null>(null);
  const [initialized, setInitialized] = useState(false);
  const tokensRef = useRef<AuthTokens | null>(tokens);
  const refreshTimeoutRef = useRef<number | null>(null);
  const refreshPromiseRef = useRef<Promise<AuthTokens | null> | null>(null);

  useEffect(() => { tokensRef.current = tokens; }, [tokens]);

  const persistTokens = useCallback((next: AuthTokens | null) => {
    setTokens(next);
  }, []);

  const clearRefreshTimeout = useCallback(() => {
    if (refreshTimeoutRef.current !== null) {
      window.clearTimeout(refreshTimeoutRef.current);
      refreshTimeoutRef.current = null;
    }
  }, []);

  const logout = useCallback(() => {
    clearRefreshTimeout();
    // Fire-and-forget server logout
    try {
      fetch(`${API_BASE_URL}/logout`, { method: "POST", credentials: "include" }).catch(() => {});
    } catch {}
    persistTokens(null);
    setCurrentUser(null);
  }, [clearRefreshTimeout, persistTokens]);

  const scheduleRefresh = useCallback((t: AuthTokens | null) => {
    clearRefreshTimeout();
    if (!t) return;
    const msUntilExpiry = new Date(t.accessTokenExpiresUtc).getTime() - Date.now();
    const safetyWindowMs = 10_000; // Refresh 10s early
    const delay = Math.max(0, msUntilExpiry - safetyWindowMs);
    refreshTimeoutRef.current = window.setTimeout(() => {
      void refreshAccessToken();
    }, delay) as unknown as number;
  }, [clearRefreshTimeout]);

  const refreshAccessToken = useCallback(async (): Promise<AuthTokens | null> => {
    if (refreshPromiseRef.current) {
      return await refreshPromiseRef.current;
    }
    const p = (async () => {
      try {
        const updated = await refreshAuthToken();
        if (!updated) {
          // No valid refresh cookie; clear tokens
          persistTokens(null);
          return null;
        }
        const next: AuthTokens = {
          accessToken: updated.accessToken,
          accessTokenExpiresUtc: updated.accessTokenExpiresUtc,
        };
        persistTokens(next);
        scheduleRefresh(next);
        return next;
      } catch (e) {
        // On failure, log out once
        logout();
        throw e;
      } finally {
        refreshPromiseRef.current = null;
      }
    })();
    refreshPromiseRef.current = p;
    return await p;
  }, [logout, persistTokens, scheduleRefresh]);

  // Try refresh on mount; never surface errors
  useEffect(() => {
    let mounted = true;
    (async () => {
      try {
        await refreshAccessToken().catch(() => null);
      } finally {
        if (mounted) setInitialized(true);
      }
    })();
    return () => { mounted = false; };
  }, [refreshAccessToken]);

  const wrappedLogin = useCallback((nextTokens: AccessTokenDto) => {
    const t: AuthTokens = {
      accessToken: nextTokens.accessToken,
      accessTokenExpiresUtc: nextTokens.accessTokenExpiresUtc,
    };
    persistTokens(t);
    scheduleRefresh(t);
    setInitialized(true);
  }, [persistTokens, scheduleRefresh]);

  const login = wrappedLogin;

  const ensureAccessToken = useCallback(async (): Promise<string | null> => {
    const current = tokensRef.current;
    if (!current) {
      const refreshed = await refreshAccessToken();
      return refreshed?.accessToken ?? null;
    }
    const expiresAt = new Date(current.accessTokenExpiresUtc).getTime();
    const needsRefresh = expiresAt - Date.now() < 10_000;
    if (needsRefresh) {
      const refreshed = await refreshAccessToken();
      return refreshed?.accessToken ?? null;
    }
    return current.accessToken;
  }, [refreshAccessToken]);

  const refreshUser = useCallback(async () => {
    const token = await ensureAccessToken();
    if (!token) {
      setCurrentUser(null);
      return;
    }
    const me = await fetchCurrentUser(token);
    setCurrentUser(me);
  }, [ensureAccessToken]);

  useEffect(() => {
    scheduleRefresh(tokens);
  }, [tokens, scheduleRefresh]);

  useEffect(() => {
    if (tokens) { void refreshUser(); }
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
    <AuthContext.Provider value={contextValue}>{children}</AuthContext.Provider>
  );
};