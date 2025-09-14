import React, { createContext, useContext, useEffect, useState } from "react";

interface AuthContextType {
    isLoggedIn: boolean;
    login: (token: string) => void;
    logout: () => void;
}

const AuthContext = createContext<AuthContextType>({
    isLoggedIn: false,
    login: () => {},
    logout: () => {},
});

export const useAuth = () => useContext(AuthContext);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [isLoggedIn, setIsLoggedIn] = useState(!!localStorage.getItem("authToken"));

    useEffect(() => {
        setIsLoggedIn(!!localStorage.getItem("authToken"));
    }, []);

    const login = (token: string) => {
        localStorage.setItem("authToken", token);
        setIsLoggedIn(true);
    };

    const logout = () => {
        localStorage.removeItem("authToken");
        setIsLoggedIn(false);
    };

    return (
        <AuthContext.Provider value={{ isLoggedIn, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
};