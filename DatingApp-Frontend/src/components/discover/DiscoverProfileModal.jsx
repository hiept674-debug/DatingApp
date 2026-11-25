import { useEffect, useRef, useState } from "react";

import {
    FaTimes,
    FaUser,
    FaCalendarAlt,
    FaInfoCircle,
    FaShieldAlt,
    FaSpinner,
    FaExclamationCircle,
    FaRedo
} from "react-icons/fa";

import discoverService from "../../services/discoverService";
import Avatar from "../Avatar";

import "./DiscoverProfileModal.css";

export default function DiscoverProfileModal({
    profileId,
    onClose
}) {
    const [profile, setProfile] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [activeTab, setActiveTab] = useState("about");
    const [reloadKey, setReloadKey] = useState(0);

    const modalRef = useRef(null);
    const closeRef = useRef(null);
    const onCloseRef = useRef(onClose);

    useEffect(() => {
        onCloseRef.current = onClose;
    }, [onClose]);

    // Load profile from API
    useEffect(() => {
        const controller = new AbortController();

        const fetchProfile = async () => {
            setLoading(true);
            setError("");
            setProfile(null);
            setActiveTab("about");

            try {
                const data =
                    await discoverService.getProfileById(
                        profileId,
                        controller.signal
                    );

                if (!controller.signal.aborted) {
                    setProfile(data);
                }
            } catch (err) {
                if (controller.signal.aborted) return;

                const status = err.response?.status;

                if (status === 404) {
                    setError("Profile not found.");
                } else if (status === 403) {
                    setError("You do not have permission.");
                } else if (status === 401) {
                    setError("Your session has expired.");
                } else {
                    setError("Unable to load profile.");
                }
            } finally {
                if (!controller.signal.aborted) {
                    setLoading(false);
                }
            }
        };

        fetchProfile();

        return () => controller.abort();
    }, [profileId, reloadKey]);

    // Keyboard accessibility and scroll lock
    useEffect(() => {
        const previousFocus = document.activeElement;
        const previousOverflow =
            document.body.style.overflow;

        document.body.style.overflow = "hidden";

        closeRef.current?.focus();

        const handleKeyDown = (event) => {
            if (event.key === "Escape") {
                onCloseRef.current();
            }

            if (event.key === "Tab") {
                const elements =
                    modalRef.current?.querySelectorAll(
                        'button:not([disabled]), a[href], input, select, textarea, [tabindex="0"]'
                    );

                if (!elements?.length) return;

                const first = elements[0];
                const last = elements[elements.length - 1];

                if (
                    event.shiftKey &&
                    document.activeElement === first
                ) {
                    event.preventDefault();
                    last.focus();
                } else if (
                    !event.shiftKey &&
                    document.activeElement === last
                ) {
                    event.preventDefault();
                    first.focus();
                }
            }
        };

        document.addEventListener(
            "keydown",
            handleKeyDown
        );

        return () => {
            document.body.style.overflow =
                previousOverflow;

            document.removeEventListener(
                "keydown",
                handleKeyDown
            );

            previousFocus?.focus();
        };
    }, []);

    const formatDate = (value) => {
        if (!value) return "Not available";

        const date = new Date(value);

        if (Number.isNaN(date.getTime())) {
            return "Not available";
        }

        return date.toLocaleDateString("vi-VN");
    };

    return (
        <div
            className="dp-modal-overlay"
            onClick={onClose}
        >
            <section
                ref={modalRef}
                className="dp-modal"
                role="dialog"
                aria-modal="true"
                aria-labelledby="dp-modal-title"
                onClick={(event) => event.stopPropagation()}
            >
                {/* Modal Header */}
                <header className="dp-modal-header">
                    <h2 id="dp-modal-title">
                        Profile Details
                    </h2>

                    <button
                        ref={closeRef}
                        type="button"
                        className="dp-close-button"
                        onClick={onClose}
                        aria-label="Close profile"
                    >
                        <FaTimes />
                    </button>
                </header>

                {/* Loading */}
                {loading && (
                    <div
                        className="dp-state"
                        role="status"
                    >
                        <FaSpinner className="dp-spinner" />

                        <h3>Loading profile...</h3>

                        <p>Please wait a moment.</p>
                    </div>
                )}

                {/* Error */}
                {!loading && error && (
                    <div
                        className="dp-state dp-error"
                        role="alert"
                    >
                        <FaExclamationCircle />

                        <h3>Something went wrong</h3>

                        <p>{error}</p>

                        <button
                            className="dp-primary-button"
                            onClick={() =>
                                setReloadKey((prev) => prev + 1)
                            }
                        >
                            <FaRedo />
                            Try again
                        </button>
                    </div>
                )}

                {/* Profile Content */}
                {!loading && !error && profile && (
                    <>
                        <div className="dp-cover">
                            <div className="dp-decoration dp-circle-one" />
                            <div className="dp-decoration dp-circle-two" />
                            <div className="dp-decoration dp-circle-three" />
                        </div>

                        <div className="dp-identity">
                            <Avatar
                                avatarUrl={profile.avatarUrl}
                                fullName={profile.fullName}
                                size={120}
                                className="dp-avatar"
                            />

                            <h2>{profile.fullName}</h2>

                            <span className="dp-profile-label">
                                <FaUser />
                                Demo profile
                            </span>

                            <p className="dp-member-since">
                                <FaCalendarAlt />

                                Member since{" "}
                                {formatDate(profile.createdAt)}
                            </p>
                        </div>

                        {/* Navigation */}
                        <div
                            className="dp-tabs"
                            role="group"
                            aria-label="Profile sections"
                        >
                            <button
                                className={
                                    activeTab === "about"
                                        ? "dp-tab active"
                                        : "dp-tab"
                                }
                                aria-pressed={activeTab === "about"}
                                onClick={() => setActiveTab("about")}
                            >
                                <FaUser />
                                About me
                            </button>

                            <button
                                className={
                                    activeTab === "information"
                                        ? "dp-tab active"
                                        : "dp-tab"
                                }
                                aria-pressed={
                                    activeTab === "information"
                                }
                                onClick={() =>
                                    setActiveTab("information")
                                }
                            >
                                <FaInfoCircle />
                                Information
                            </button>
                        </div>

                        {/* About Tab */}
                        <div className="dp-content">
                            {activeTab === "about" && (
                                <section className="dp-panel">
                                    <h3>About me</h3>

                                    <p className="dp-bio">
                                        {profile.bio?.trim() ||
                                            "No biography available."}
                                    </p>
                                </section>
                            )}

                            {/* Information Tab */}
                            {activeTab === "information" && (
                                <section className="dp-panel">
                                    <h3>Profile information</h3>

                                    <InfoRow
                                        icon={<FaUser />}
                                        label="Full name"
                                        value={profile.fullName}
                                    />

                                    <InfoRow
                                        icon={<FaCalendarAlt />}
                                        label="Member since"
                                        value={formatDate(
                                            profile.createdAt
                                        )}
                                    />

                                    <InfoRow
                                        icon={<FaInfoCircle />}
                                        label="Biography"
                                        value={
                                            profile.bio?.trim()
                                                ? "Available"
                                                : "Not updated"
                                        }
                                    />
                                </section>
                            )}

                            {/* Privacy Notice */}
                            <div className="dp-privacy">
                                <FaShieldAlt />

                                <div>
                                    <strong>Privacy protection</strong>

                                    <p>
                                        Private account information
                                        is protected.
                                    </p>
                                </div>
                            </div>
                        </div>
                    </>
                )}

                {/* Modal Footer */}
                <footer className="dp-footer">
                    <button
                        className="dp-primary-button"
                        onClick={onClose}
                    >
                        Close profile
                    </button>
                </footer>
            </section>
        </div>
    );
}

function InfoRow({ icon, label, value }) {
    return (
        <div className="dp-info-row">
            <div className="dp-info-icon">
                {icon}
            </div>

            <div className="dp-info-text">
                <span>{label}</span>
                <strong>{value || "Not available"}</strong>
            </div>
        </div>
    );
}