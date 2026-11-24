
import { useEffect, useState } from "react";

function getTokenExpiration() {
    const token =
        localStorage.getItem("token") ||
        sessionStorage.getItem("token");

    if (!token) return null;

    try {
        const payload = token.split(".")[1];

        const base64 = payload
            .replace(/-/g, "+")
            .replace(/_/g, "/");

        const decoded = JSON.parse(
            atob(base64)
        );

        if (!decoded.exp) return null;

        return decoded.exp * 1000;
    } catch {
        return null;
    }
}

function SessionInfo() {
    const [expiration, setExpiration] = useState(
        getTokenExpiration
    );

    const [remaining, setRemaining] = useState("");

    useEffect(() => {
        const updateTime = () => {
            if (!expiration) {
                setRemaining("No active session");
                return;
            }

            const diff = expiration - Date.now();

            if (diff <= 0) {
                setRemaining("Session expired");
                return;
            }

            const hours = Math.floor(
                diff / 3600000
            );

            const minutes = Math.floor(
                (diff % 3600000) / 60000
            );

            const seconds = Math.floor(
                (diff % 60000) / 1000
            );

            setRemaining(
                `${hours}h ${minutes}m ${seconds}s`
            );
        };

        updateTime();

        const interval = setInterval(
            updateTime,
            1000
        );

        return () => clearInterval(interval);
    }, [expiration]);

    return (
        <div className="card shadow-sm">
            <div className="card-body">
                <h5 className="card-title">
                    Session Information
                </h5>

                <p>
                    <strong>Status:</strong>{" "}
                    Active
                </p>

                <p>
                    <strong>Expires in:</strong>{" "}
                    {remaining}
                </p>

                <p>
                    <strong>Remember Me:</strong>{" "}
                    {localStorage.getItem("token")
                        ? "Enabled"
                        : "Disabled"}
                </p>
            </div>
        </div>
    );
}

export default SessionInfo;
