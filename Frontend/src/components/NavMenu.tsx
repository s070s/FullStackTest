import { useState, useRef } from "react";
import { useNavigate } from "react-router-dom";
import Button from "./Button";
import { useAuth } from "../utils/contexts/AuthContext";
import { uploadProfilePhoto, fetchCurrentUser, API_BASE_URL } from "../utils/api/api";

/**
 * NavMenu
 * - Auth-aware navigation panel: shows Login/Register when unauthenticated,
 *   or role-specific links + Logout when authenticated.
 * - Displays user avatar (click-to-upload) and refreshes currentUser after a successful upload.
 * - Maintains a compact/hidden state for a collapsible UI; uses react-router for navigation.
 * - Relies on AuthContext for user/token actions and keeps presentation responsibilities to CSS.
 */
const NavMenu: React.FC = () => {
    const [hidden, setHidden] = useState(false);
    const navigate = useNavigate();
    const { isLoggedIn, logout, currentUser, setCurrentUser, ensureAccessToken } = useAuth();
    const fileInputRef = useRef<HTMLInputElement>(null);

    const handleNavigate = (path: string) => {
        setHidden(true);
        navigate(path);
    };

    const handleLogout = () => {
        logout();
        navigate("/login");
    };
    const handlePhotoClick = () => {
        if (fileInputRef.current) {
            fileInputRef.current.value = "";
            fileInputRef.current.click();
        }
    };

    // When a file is selected, upload it and refresh user info
    const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
        if (!currentUser) return;
        const file = e.target.files?.[0];
        if (!file) return;
        try {
            const accessToken = await ensureAccessToken();
            if (!accessToken) return;
            await uploadProfilePhoto(accessToken, currentUser.id, file);
            // Refresh user info to get new photo URL
            const updatedUser = await fetchCurrentUser(accessToken);
            setCurrentUser(updatedUser);
        } catch {
            alert("Admins do not have permission to upload profile photos.");
        }
    };

    return (
        <div className="nav-menu-wrapper">
            <nav
                className={`nav-menu${hidden ? " nav-menu--hidden" : ""}`}
            >
                <div className="grid-container">
                    {isLoggedIn && currentUser && (
                        <div>
                            <img
                                src={
                                    currentUser.role === "Client" && currentUser.clientProfile?.profilePhotoUrl
                                        ? `${API_BASE_URL}${currentUser.clientProfile.profilePhotoUrl}?t=${Date.now()}`
                                        : currentUser.role === "Trainer" && currentUser.trainerProfile?.profilePhotoUrl
                                        ? `${API_BASE_URL}${currentUser.trainerProfile.profilePhotoUrl}?t=${Date.now()}`
                                        : "/default-avatar.png"
                                }
                                alt="Profile"
                                width={40}
                                height={40}
                                style={{ borderRadius: "50%", objectFit: "cover", cursor: "pointer" }}
                                onClick={handlePhotoClick}
                                title="Click to upload new photo"
                            />
                            <input
                                type="file"
                                accept="image/png,image/jpeg"
                                style={{ display: "none" }}
                                ref={fileInputRef}
                                onChange={handleFileChange}
                            />
                        </div>
                    )}
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
                            {currentUser?.role === "Client" && (
                                <Button onClick={() => handleNavigate("/trainers")}>
                                    Find Trainers
                                </Button>
                            )}
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