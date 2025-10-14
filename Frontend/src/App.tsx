import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import NavMenu from "./components/NavMenu";
import RegisterPage from "./pages/RegisterPage";
import LoginPage from "./pages/LoginPage";
import WelcomePage from "./pages/WelcomePage";
import Dashboard from "./pages/Dashboard";
import TrainerBrowser from "./pages/Client/TrainerBrowser";
import { useAuth } from "./utils/contexts/AuthContext";
import LoadingSpinner from './components/LoadingSpinner';

const App: React.FC = () => {
    const { isLoggedIn, initialized, currentUser } = useAuth();

    // Wait until AuthProvider finishes the initial silent refresh check.
    // This prevents immediate redirects to /login on page reload while the cookie-based refresh is in-flight.
    if (!initialized) {
        return null; // or return a small spinner component if you prefer
    }

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
                <Route
                    path="/trainers"
                    element={
                        !isLoggedIn
                            ? <Navigate to="/login" replace />
                            : !currentUser
                                ? <LoadingSpinner />
                                : currentUser.role === "Client"
                                    ? <TrainerBrowser />
                                    : <Navigate to="/dashboard" replace />
                    }
                />
                <Route path="*" element={<Navigate to={isLoggedIn ? "/dashboard" : "/login"} replace />} />
            </Routes>
        </>
    );
};

export default App;
