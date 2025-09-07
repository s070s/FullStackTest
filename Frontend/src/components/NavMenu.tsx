import { useState } from "react";
import { useNavigate } from "react-router-dom";
import Button from "./Button";
import { useAuth } from "../contexts/AuthContext";

const NavMenu: React.FC = () => {
    const [hidden, setHidden] = useState(false);
    const navigate = useNavigate();
    const { isLoggedIn, logout } = useAuth();

    const handleNavigate = (path: string) => {
        setHidden(true);
        navigate(path);
    };

    return (
        <nav
            className={`nav-menu${hidden ? " nav-menu--hidden" : ""}`}
            onMouseEnter={() => setHidden(false)}
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
                        <Button onClick={logout}>
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
                    &gt;
                </div>
            )}
        </nav>
    );
}

export default NavMenu;