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
import type { TokenPairDto, UserDto } from "../data/userdtos";
import { fetchCurrentUser, refreshAuthToken } from "../api/api";

interface AuthContextType {
  isLoggedIn: boolean;
  token: string | null;
  refreshToken: string | null;
  currentUser: UserDto | null;
  setCurrentUser: (user: UserDto | null) => void;
  login: (tokens: TokenPairDto) => void;
  logout: () => void;
  refreshUser: () => Promise<void>;
  ensureAccessToken: () => Promise<string | null>;
}

type AuthTokens = TokenPairDto;

const STORAGE_KEY = "auth.tokens";

const loadStoredTokens = (): AuthTokens | null => {
  if (typeof window === "undefined" || !window.localStorage) {
    return null;
  }

  const raw = window.localStorage.getItem(STORAGE_KEY);
  if (!raw) return null;
  try {
    return JSON.parse(raw) as AuthTokens;
  } catch {
    window.localStorage.removeItem(STORAGE_KEY);
    return null;
  }
};

const AuthContext = createContext<AuthContextType>({
  isLoggedIn: false,
  token: null,
  refreshToken: null,
  currentUser: null,
  setCurrentUser: () => { },
  login: () => { },
  logout: () => { },
  refreshUser: async () => { },
  ensureAccessToken: async () => null,
});

export const useAuth = () => useContext(AuthContext);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [tokens, setTokens] = useState<AuthTokens | null>(() => loadStoredTokens());
  const [currentUser, setCurrentUser] = useState<UserDto | null>(null);
  const tokensRef = useRef<AuthTokens | null>(tokens);
  const refreshTimeoutRef = useRef<number | null>(null);

  useEffect(() => {
    tokensRef.current = tokens;
  }, [tokens]);

  const persistTokens = useCallback((next: AuthTokens | null) => {
    if (typeof window !== "undefined" && window.localStorage) {
      if (next) {
        window.localStorage.setItem(STORAGE_KEY, JSON.stringify(next));
      } else {
        window.localStorage.removeItem(STORAGE_KEY);
      }
    }

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
    persistTokens(null);
    setCurrentUser(null);
  }, [clearRefreshTimeout, persistTokens]);

  const login = useCallback((nextTokens: TokenPairDto) => {
    persistTokens(nextTokens);
  }, [persistTokens]);

  const refreshAccessToken = useCallback(async (): Promise<AuthTokens | null> => {
    const current = tokensRef.current;
    if (!current?.refreshToken) {
      logout();
      return null;
    }

    try {
      const updated = await refreshAuthToken(current.refreshToken);
      persistTokens(updated);
      return updated;
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
    refreshToken: tokens?.refreshToken ?? null,
    currentUser,
    setCurrentUser,
    login,
    logout,
    refreshUser,
    ensureAccessToken,
  }), [tokens, currentUser, login, logout, refreshUser, ensureAccessToken]);

  return (
    <AuthContext.Provider value={contextValue}>
      {children}
    </AuthContext.Provider>
  );
};