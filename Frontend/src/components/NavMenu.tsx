import { useState } from "react";
import { useNavigate } from "react-router-dom";
import Button from "./Button";
import "../css/ContainerRules.css";

function NavMenu() {
    const [hidden, setHidden] = useState(false);
    const navigate = useNavigate();

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
                <Button onClick={() => handleNavigate("/login")}>
                    Login
                </Button>
                <Button onClick={() => handleNavigate("/register")}>
                    Register
                </Button>

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