// Context to manage authentication state and token globally
import React, { createContext, useContext, useEffect, useState } from "react";
import type { UserDto } from "../data/userdtos";
import { fetchCurrentUser } from "../api/api";


interface AuthContextType {
  isLoggedIn: boolean;
  token: string | null;
  currentUser: UserDto | null;
  setCurrentUser: (user: UserDto | null) => void;
  login: (token: string) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType>({
  isLoggedIn: false,
  token: null,
  currentUser: null,
  setCurrentUser: () => { },
  login: () => { },
  logout: () => { },
});

export const useAuth = () => useContext(AuthContext);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [isLoggedIn, setIsLoggedIn] = useState(!!localStorage.getItem("authToken"));
  const [token, setToken] = useState<string | null>(localStorage.getItem("authToken"));
  const [currentUser, setCurrentUser] = useState<UserDto | null>(null);

  useEffect(() => {
    setIsLoggedIn(!!localStorage.getItem("authToken"));
  }, []);

  useEffect(() => {
    const getCurrentUser = async () => {
      if (!token) {
        setCurrentUser(null);
        return;
      }
      try {
        const user = await fetchCurrentUser(token);
        setCurrentUser(user);
      } catch {
        setCurrentUser(null);
      }
    };
    getCurrentUser();
  }, [token]);

  const login = (token: string) => {
    localStorage.setItem("authToken", token);
    setToken(token);
    setIsLoggedIn(true);
  };
  const logout = () => {
    localStorage.removeItem("authToken");
    setToken(null);
    setIsLoggedIn(false);
    setCurrentUser(null);
  };

  return (
    <AuthContext.Provider value={{ isLoggedIn, token, currentUser, setCurrentUser, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};