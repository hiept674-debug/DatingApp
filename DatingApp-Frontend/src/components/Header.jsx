
import { Link, useLocation } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { FaBars } from "react-icons/fa";
import Avatar from "./Avatar";
function Header({ onMenuClick }) {
    const { user } = useAuth();
    const location = useLocation();

    const pageTitles = {
        "/home": "Dashboard",
        "/discover": "Discover",
        "/matches": "Matches",
        "/chat": "Messages",
        "/profile": "My Profile",
    };

    const initial =
        user?.fullName?.trim()?.charAt(0)?.toUpperCase() || "H";

    return (
        <header className="main-header">
            <div className="header-left">
                <button
                    type="button"
                    className="mobile-menu-button"
                    onClick={onMenuClick}
                    aria-label="Open menu"
                >
                    <FaBars />
                </button>

                <h2>
                    {pageTitles[location.pathname] || "DatingApp"}
                </h2>
            </div>

            <div className="header-right">
                <Link to="/profile" className="header-user">
                    <Avatar
                        avatarUrl={user?.avatarUrl}
                        fullName={user?.fullName}
                        size={42}
                    />

                    <div className="header-user-info">
                        <strong>{user?.fullName || "User"}</strong>
                        <small>My account</small>
                    </div>
                </Link>
            </div>
        </header>
    );
}

export default Header;
