
import { NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

import {
    FaHeart,
    FaHome,
    FaCompass,
    FaComments,
    FaUser,
    FaSignOutAlt,
} from "react-icons/fa";

function Sidebar({ open, onClose }) {
    const { logout } = useAuth();
    const navigate = useNavigate();

    const menuItems = [
        { path: "/home", label: "Home", icon: FaHome },
        { path: "/discover", label: "Discover", icon: FaCompass },
        { path: "/matches", label: "Matches", icon: FaHeart },
        { path: "/chat", label: "Messages", icon: FaComments },
        { path: "/profile", label: "My Profile", icon: FaUser },
    ];

    const handleLogout = async () => {
        await logout();
        navigate("/login", { replace: true });
    };

    return (
        <aside className={`main-sidebar ${open ? "sidebar-open" : ""}`}>
            <NavLink
                to="/home"
                className="sidebar-brand"
                onClick={onClose}
            >
                <FaHeart />
                <span>DatingApp</span>
            </NavLink>

            <p className="sidebar-label">MAIN MENU</p>

            <nav className="sidebar-nav">
                {menuItems.map(({ path, label, icon: Icon }) => (
                    <NavLink
                        key={path}
                        to={path}
                        onClick={onClose}
                        className={({ isActive }) =>
                            `sidebar-link ${isActive ? "active" : ""}`
                        }
                    >
                        <Icon />
                        <span>{label}</span>
                    </NavLink>
                ))}
            </nav>

            <div className="sidebar-bottom">
                <button
                    type="button"
                    className="sidebar-link logout-link"
                    onClick={handleLogout}
                >
                    <FaSignOutAlt />
                    <span>Logout</span>
                </button>

                <small>© 2026 DatingApp</small>
            </div>
        </aside>
    );
}

export default Sidebar;
