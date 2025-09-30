import React from 'react';
import {Routes, Route, Navigate } from 'react-router-dom';
import NavMenu from "./components/NavMenu";
import RegisterPage from "./pages/RegisterPage";
import LoginPage from "./pages/LoginPage";
import WelcomePage from "./pages/WelcomePage";
import Dashboard from "./pages/Dashboard";
import { useAuth } from "./utils/contexts/AuthContext";

const App: React.FC = () => {
    const { isLoggedIn } = useAuth();

    return (
        <>

            <NavMenu />
            <hr />
            <Routes>
                <Route path="/" element={<  WelcomePage />} />
                <Route path="/login" element={<LoginPage />} />
                <Route path="/register" element={<RegisterPage />} />
                <Route
                    path="/dashboard"
                    element={
                        isLoggedIn ? <Dashboard /> : <Navigate to="/login" replace />
                    }
                />
                <Route path="*" element={<Navigate to={isLoggedIn ? "/dashboard" : "/login"} replace />} />
            </Routes>
        </>
    );
};

export default App;
