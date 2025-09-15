import { useState } from "react";
import { useNavigate } from "react-router-dom";
import Button from "./Button";
import { useAuth } from "../utils/contexts/AuthContext";


const NavMenu: React.FC = () => {
    const [hidden, setHidden] = useState(false);
    const navigate = useNavigate();
    const { isLoggedIn, logout } = useAuth();




    const handleNavigate = (path: string) => {
        setHidden(true);
        navigate(path);
    };

    const handleLogout = () => {
        logout();
        navigate("/login");
    };

    return (
        <div className="nav-menu-wrapper">
            <nav
                className={`nav-menu${hidden ? " nav-menu--hidden" : ""}`}
            >
                <div className="grid-container">
                    {!isLoggedIn && (
                        <>
                            <Button onClick={() => handleNavigate("/login")}>
                                Login
                            </Button>
                            <Button onClick={() => handleNavigate("/register")}>
                                Register
                            </Button>
                        </>
                    )}
                    {isLoggedIn && (
                        <>
                            <Button onClick={() => handleNavigate("/dashboard")}>
                                Dashboard
                            </Button>
                            <Button onClick={handleLogout}>
                                Logout
                            </Button>
                        </>
                    )}
                </div>
                {hidden && (
                    <div
                        className="nav-menu__handle"
                        onMouseEnter={() => setHidden(false)}
                    >
                        <i className="fas fa-arrow-right"></i>
                    </div>
                )}
            </nav>
        </div>
    );
}

export default NavMenu;