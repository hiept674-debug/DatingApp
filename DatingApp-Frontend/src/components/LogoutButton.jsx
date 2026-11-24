
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

function LogoutButton() {
    const { logout } = useAuth();
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);

    const handleLogout = async () => {
        setLoading(true);

        await logout();

        navigate("/login", {
            replace: true,
        });
    };

    return (
        <button
            className="btn btn-outline-danger"
            onClick={handleLogout}
            disabled={loading}
        >
            {loading ? "Signing out..." : "Logout"}
        </button>
    );
}

export default LogoutButton;
