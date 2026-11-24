
import { Link } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

import {
    FaUser,
    FaCompass,
    FaHeart,
    FaComments,
    FaArrowRight,
    FaShieldAlt,
} from "react-icons/fa";

import "./Home.css";

function Home() {
    const { user } = useAuth();

    const features = [
        {
            title: "My Profile",
            description: "View and manage your personal information.",
            icon: FaUser,
            path: "/profile",
            color: "pink",
        },
        {
            title: "Discover",
            description: "Explore sample profiles in the demo.",
            icon: FaCompass,
            path: "/discover",
            color: "purple",
        },
        {
            title: "Matches",
            description: "View the sample connection module.",
            icon: FaHeart,
            path: "/matches",
            color: "orange",
        },
        {
            title: "Messages",
            description: "Explore the messaging module.",
            icon: FaComments,
            path: "/chat",
            color: "blue",
        },
    ];

    return (
        <div className="home-dashboard">
            <section className="welcome-banner">
                <div className="welcome-content">
                    <span className="welcome-label">
                        YOUR DASHBOARD
                    </span>

                    <h1>
                        Welcome back, {user?.fullName || "User"}!
                    </h1>

                    <p>
                        Manage your profile and explore the
                        DatingApp demo workspace.
                    </p>

                    <Link
                        to="/profile"
                        className="welcome-button"
                    >
                        View my profile
                        <FaArrowRight />
                    </Link>
                </div>

                <FaHeart className="welcome-decoration" />
            </section>

            <section className="home-section">
                <div className="section-heading">
                    <h2>Quick Actions</h2>
                    <p>Everything you need in one place.</p>
                </div>

                <div className="feature-grid">
                    {features.map(
                        ({ title, description, icon: Icon, path, color }) => (
                            <Link
                                key={path}
                                to={path}
                                className="feature-card"
                            >
                                <div className={`feature-icon ${color}`}>
                                    <Icon />
                                </div>

                                <h3>{title}</h3>
                                <p>{description}</p>

                                <span className="feature-link">
                                    View details <FaArrowRight />
                                </span>
                            </Link>
                        )
                    )}
                </div>
            </section>

            <div className="home-bottom-grid">
                <section className="home-panel">
                    <h2>Account Overview</h2>

                    <div className="account-row">
                        <span>Full name</span>
                        <strong>{user?.fullName || "Not provided"}</strong>
                    </div>

                    <div className="account-row">
                        <span>Email</span>
                        <strong>{user?.email || "Not provided"}</strong>
                    </div>

                    <div className="account-row">
                        <span>Phone number</span>
                        <strong>{user?.phone || "Not provided"}</strong>
                    </div>

                    <Link to="/profile" className="panel-link">
                        View profile <FaArrowRight />
                    </Link>
                </section>

                <section className="home-panel">
                    <div className="security-icon">
                        <FaShieldAlt />
                    </div>

                    <h2>Account & Privacy</h2>

                    <p>
                        Keep your account information private
                        and use sample profiles for testing.
                    </p>

                    <div className="session-status">
                        <span className="status-dot" />
                        Signed in
                    </div>

                    <Link to="/profile" className="panel-link">
                        Manage account <FaArrowRight />
                    </Link>
                </section>
            </div>
        </div>
    );
}

export default Home;
