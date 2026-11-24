
import { useEffect, useState } from "react";
import { useAuth } from "../context/AuthContext";
import userService from "../services/userService";
import Avatar from "../components/Avatar";
import AvatarUploadModal from "../components/AvatarUploadModal";

import { FaCamera, FaTrashAlt } from "react-icons/fa";
import {
    FaUser,
    FaEdit,
    FaCheck,
    FaTimes,
    FaEnvelope,
    FaPhone,
    FaCalendarAlt,
    FaMapMarkerAlt,
    FaVenusMars,
    FaShieldAlt,
    FaSpinner,
    FaExclamationCircle,
} from "react-icons/fa";

import "./Profile.css";

function Profile() {
    const { updateUser } = useAuth();

    const [profile, setProfile] = useState(null);
    const [form, setForm] = useState({});

    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [editing, setEditing] = useState(false);

    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");
    const [errors, setErrors] = useState({});

    const [avatarModalOpen, setAvatarModalOpen] =
        useState(false);

    const [deletingAvatar, setDeletingAvatar] =
        useState(false);

    // Chuyển dữ liệu API sang dữ liệu form
    const createForm = (data) => ({
        fullName: data.fullName || "",
        dateOfBirth: data.dateOfBirth?.split("T")[0] || "",
        gender: data.gender || "",
        city: data.city || "",
        bio: data.bio || "",
    });

    // Xử lý thông báo lỗi API
    const getErrorMessage = (err) => {
        const data = err.response?.data;

        if (data?.errors) {
            return Object.values(data.errors)
                .flat()
                .join(" ");
        }

        return (
            data?.message ||
            data?.title ||
            err.message ||
            "Something went wrong."
        );
    };

    // Lấy thông tin hồ sơ
    const fetchProfile = async () => {
        try {
            setLoading(true);
            setError("");

            const data = await userService.getMyProfile();

            setProfile(data);
            setForm(createForm(data));
        } catch (err) {
            setError(getErrorMessage(err));
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchProfile();
    }, []);

    // Thay đổi input
    const handleChange = (e) => {
        const { name, value } = e.target;

        setForm((previous) => ({
            ...previous,
            [name]: value,
        }));

        setErrors((previous) => ({
            ...previous,
            [name]: "",
        }));

        setError("");
        setSuccess("");
    };

    // Kiểm tra dữ liệu
    const validate = () => {
        const newErrors = {};

        if (!form.fullName.trim()) {
            newErrors.fullName = "Full name is required.";
        } else if (form.fullName.length > 100) {
            newErrors.fullName = "Maximum 100 characters.";
        }

        if (!form.dateOfBirth) {
            newErrors.dateOfBirth = "Date of birth is required.";
        } else {
            const today = new Date();
            const birthday = new Date(
                form.dateOfBirth + "T00:00:00"
            );

            if (
                Number.isNaN(birthday.getTime()) ||
                birthday > today
            ) {
                newErrors.dateOfBirth = "Invalid date of birth.";
            }
        }

        if (!form.gender.trim()) {
            newErrors.gender = "Gender is required.";
        } else if (form.gender.length > 20) {
            newErrors.gender = "Maximum 20 characters.";
        }

        if (!form.city.trim()) {
            newErrors.city = "City is required.";
        } else if (form.city.length > 100) {
            newErrors.city = "Maximum 100 characters.";
        }

        if (form.bio.length > 500) {
            newErrors.bio = "Maximum 500 characters.";
        }

        setErrors(newErrors);

        return Object.keys(newErrors).length === 0;
    };

    // Bắt đầu chỉnh sửa
    const handleEdit = () => {
        setForm(createForm(profile));
        setEditing(true);
        setSuccess("");
        setError("");
        setErrors({});
    };

    // Hủy chỉnh sửa
    const handleCancel = () => {
        setForm(createForm(profile));
        setEditing(false);
        setErrors({});
        setError("");
    };

    // Lưu thông tin
    const handleSubmit = async (e) => {
        e.preventDefault();

        setError("");
        setSuccess("");

        if (!validate()) return;

        try {
            setSaving(true);

            const payload = {
                fullName: form.fullName.trim(),
                dateOfBirth: form.dateOfBirth,
                gender: form.gender.trim(),
                city: form.city.trim(),
                bio: form.bio.trim(),
            };

            const updatedProfile =
                await userService.updateMyProfile(payload);

            setProfile(updatedProfile);
            setForm(createForm(updatedProfile));

            // Đồng bộ Header và Home
            updateUser(updatedProfile);

            setEditing(false);
            setSuccess("Your profile has been updated successfully.");
        } catch (err) {
            setError(getErrorMessage(err));
        } finally {
            setSaving(false);
        }
    };

    const formatDate = (value) => {
        if (!value) return "Not provided";

        const date = new Date(
            value.split("T")[0] + "T00:00:00"
        );

        return date.toLocaleDateString("vi-VN");
    };

    const handleAvatarSuccess = (result) => {
        setProfile((previous) => ({
            ...previous,
            avatarUrl: result.avatarUrl,
        }));

        updateUser({
            avatarUrl: result.avatarUrl,
        });

        setAvatarModalOpen(false);

        setError("");
        setSuccess("Profile picture updated successfully.");
    };
    const handleDeleteAvatar = async () => {
        if (deletingAvatar) return;

        const confirmed = window.confirm(
            "Are you sure you want to remove your profile picture?"
        );

        if (!confirmed) return;

        try {
            setDeletingAvatar(true);

            setError("");
            setSuccess("");

            await userService.deleteAvatar();

            setProfile((previous) => ({
                ...previous,
                avatarUrl: null,
            }));

            updateUser({
                avatarUrl: null,
            });

            setSuccess("Profile picture removed successfully.");
        } catch (err) {
            setError(
                err.response?.data?.message ||
                "Unable to remove profile picture."
            );
        } finally {
            setDeletingAvatar(false);
        }
    };

    // Loading State
    if (loading) {
        return (
            <div className="profile-loading">
                <FaSpinner className="profile-spinner" />
                <p>Loading your profile...</p>
            </div>
        );
    }

    // Error State
    if (!profile) {
        return (
            <div className="profile-empty">
                <FaExclamationCircle />

                <h2>Unable to load profile</h2>

                <p>{error}</p>

                <button
                    type="button"
                    className="profile-btn primary"
                    onClick={fetchProfile}
                >
                    Try again
                </button>
            </div>
        );
    }

    const initial =
        profile.fullName?.trim().charAt(0).toUpperCase() || "U";

    const completion = Math.min(
        100,
        Math.max(0, Number(profile.profileCompletion) || 0)
    );

    return (
        <div className="profile-page">

            {/* Page Heading */}
            <div className="profile-page-heading">
                <h1>My Profile</h1>
                <p>View and manage your account information.</p>
            </div>

            {/* Success Message */}
            {success && (
                <div
                    className="profile-alert success"
                    role="status"
                >
                    <FaCheck />
                    <span>{success}</span>

                    <button
                        type="button"
                        aria-label="Dismiss notification"
                        onClick={() => setSuccess("")}
                    >
                        <FaTimes />
                    </button>
                </div>
            )}

            {/* Error Message */}
            {error && (
                <div
                    className="profile-alert danger"
                    role="alert"
                >
                    <FaExclamationCircle />
                    <span>{error}</span>
                </div>
            )}

            {/* Profile Header */}
            <section className="profile-header-card">

                <div className="profile-cover">
                    <FaUser className="profile-cover-icon" />
                </div>

                <div className="profile-header-content">


                    <div className="profile-avatar-wrapper">

                        <Avatar
                            avatarUrl={profile.avatarUrl}
                            fullName={profile.fullName}
                            size={112}
                            className="profile-avatar-image"
                        />

                        <button
                            type="button"
                            className="profile-avatar-camera"
                            onClick={() => setAvatarModalOpen(true)}
                            disabled={deletingAvatar || editing}
                            aria-label="Change profile picture"
                            title="Change profile picture"
                        >
                            <FaCamera />
                        </button>

                    </div>


                    <div className="profile-identity">
                        <h2>{profile.fullName}</h2>
                        <p>{profile.email}</p>

                        <span className="profile-account-label">
                            Personal account
                        </span>
                    </div>

                    {!editing && (
                        <button
                            type="button"
                            className="profile-btn primary"
                            onClick={handleEdit}
                        >
                            <FaEdit />
                            Edit Profile
                        </button>
                    )}

                </div>
                
<div className="profile-avatar-actions">

    <div className="profile-avatar-description">
        
    </div>

    <div className="profile-avatar-buttons">

        <button
            type="button"
            className="profile-btn secondary"
            onClick={() => setAvatarModalOpen(true)}
            disabled={deletingAvatar || editing}
        >
            <FaCamera />
            Change Avatar
        </button>

        {profile.avatarUrl && (
            <button
                type="button"
                className="profile-btn danger"
                onClick={handleDeleteAvatar}
                disabled={deletingAvatar || editing}
            >
                {deletingAvatar ? (
                    <FaSpinner className="profile-spinner" />
                ) : (
                    <FaTrashAlt />
                )}

                {deletingAvatar
                    ? "Removing..."
                    : "Remove"}
            </button>
        )}

    </div>
</div>

            </section>

            <div className="profile-content-grid">

                {/* Personal Information */}
                <section className="profile-panel">

                    <div className="profile-panel-heading">
                        <h2>
                            <FaUser />
                            Personal Information
                        </h2>

                        <p>Manage your private account details.</p>
                    </div>

                    {editing ? (
                        <form
                            className="profile-edit-form"
                            onSubmit={handleSubmit}
                            noValidate
                        >

                            <div className="profile-form-grid">

                                <div className="profile-field">
                                    <label htmlFor="fullName">
                                        Full Name
                                    </label>

                                    <input
                                        id="fullName"
                                        name="fullName"
                                        value={form.fullName}
                                        onChange={handleChange}
                                        maxLength={100}
                                        disabled={saving}
                                        className={
                                            errors.fullName
                                                ? "invalid"
                                                : ""
                                        }
                                    />

                                    {errors.fullName && (
                                        <small className="profile-error">
                                            {errors.fullName}
                                        </small>
                                    )}
                                </div>

                                <div className="profile-field">
                                    <label>Email Address</label>

                                    <input
                                        value={profile.email}
                                        readOnly
                                        aria-label="Email address"
                                    />

                                    <small className="profile-hint">
                                        Email cannot be changed here.
                                    </small>
                                </div>

                                <div className="profile-field">
                                    <label>Phone Number</label>

                                    <input
                                        value={profile.phone || ""}
                                        readOnly
                                        aria-label="Phone number"
                                    />
                                </div>

                                <div className="profile-field">
                                    <label htmlFor="dateOfBirth">
                                        Date of Birth
                                    </label>

                                    <input
                                        id="dateOfBirth"
                                        type="date"
                                        name="dateOfBirth"
                                        value={form.dateOfBirth}
                                        onChange={handleChange}
                                        disabled={saving}
                                    />

                                    {errors.dateOfBirth && (
                                        <small className="profile-error">
                                            {errors.dateOfBirth}
                                        </small>
                                    )}
                                </div>

                                <div className="profile-field">
                                    <label htmlFor="gender">
                                        Gender
                                    </label>

                                    <input
                                        id="gender"
                                        name="gender"
                                        value={form.gender}
                                        onChange={handleChange}
                                        maxLength={20}
                                        disabled={saving}
                                    />

                                    {errors.gender && (
                                        <small className="profile-error">
                                            {errors.gender}
                                        </small>
                                    )}
                                </div>

                                <div className="profile-field">
                                    <label htmlFor="city">
                                        City
                                    </label>

                                    <input
                                        id="city"
                                        name="city"
                                        value={form.city}
                                        onChange={handleChange}
                                        maxLength={100}
                                        disabled={saving}
                                    />

                                    {errors.city && (
                                        <small className="profile-error">
                                            {errors.city}
                                        </small>
                                    )}
                                </div>

                            </div>

                            <div className="profile-field">
                                <label htmlFor="bio">Bio</label>

                                <textarea
                                    id="bio"
                                    name="bio"
                                    rows={5}
                                    value={form.bio}
                                    onChange={handleChange}
                                    maxLength={500}
                                    disabled={saving}
                                    placeholder="Write a short description..."
                                />

                                <div className="profile-character-count">
                                    {form.bio.length}/500
                                </div>

                                {errors.bio && (
                                    <small className="profile-error">
                                        {errors.bio}
                                    </small>
                                )}
                            </div>

                            <div className="profile-form-actions">

                                <button
                                    type="button"
                                    className="profile-btn secondary"
                                    onClick={handleCancel}
                                    disabled={saving}
                                >
                                    <FaTimes />
                                    Cancel
                                </button>

                                <button
                                    type="submit"
                                    className="profile-btn primary"
                                    disabled={saving}
                                >
                                    {saving ? (
                                        <FaSpinner className="profile-spinner" />
                                    ) : (
                                        <FaCheck />
                                    )}

                                    {saving
                                        ? "Saving..."
                                        : "Save Changes"}
                                </button>

                            </div>
                        </form>
                    ) : (
                        <div className="profile-information">

                            <InfoRow
                                icon={FaUser}
                                label="Full Name"
                                value={profile.fullName}
                            />

                            <InfoRow
                                icon={FaEnvelope}
                                label="Email Address"
                                value={profile.email}
                            />

                            <InfoRow
                                icon={FaPhone}
                                label="Phone Number"
                                value={profile.phone}
                            />

                            <InfoRow
                                icon={FaCalendarAlt}
                                label="Date of Birth"
                                value={formatDate(profile.dateOfBirth)}
                            />

                            <InfoRow
                                icon={FaVenusMars}
                                label="Gender"
                                value={profile.gender}
                            />

                            <InfoRow
                                icon={FaMapMarkerAlt}
                                label="City"
                                value={profile.city}
                            />

                            <div className="profile-bio">
                                <h3>Bio</h3>

                                <p>
                                    {profile.bio || "No bio provided."}
                                </p>
                            </div>

                        </div>
                    )}

                </section>

                {/* Right Sidebar */}
                <aside className="profile-right-column">

                    {/* Completion */}
                    <section className="profile-panel completion-panel">

                        <h2>Account Completion</h2>

                        <div
                            className="profile-progress-circle"
                            style={{
                                "--progress": `${completion}%`,
                            }}
                            role="progressbar"
                            aria-valuenow={completion}
                            aria-valuemin={0}
                            aria-valuemax={100}
                            aria-label="Account completion"
                        >
                            <div className="profile-progress-inner">
                                <strong>{completion}%</strong>
                                <span>Completed</span>
                            </div>
                        </div>

                        <p>
                            {completion === 100
                                ? "Your account information is complete."
                                : "Review your account information."}
                        </p>

                    </section>

                    {/* Account Details */}
                    <section className="profile-panel">

                        <h2>Account Details</h2>

                        <div className="profile-detail-item">
                            <span>Member since</span>
                            <strong>
                                {formatDate(profile.createdAt)}
                            </strong>
                        </div>

                        <div className="profile-detail-item">
                            <span>Last updated</span>
                            <strong>
                                {profile.updatedAt
                                    ? formatDate(profile.updatedAt)
                                    : "Not updated"}
                            </strong>
                        </div>

                    </section>

                    {/* Privacy */}
                    <section className="profile-panel">

                        <div className="profile-security-icon">
                            <FaShieldAlt />
                        </div>

                        <h2>Account Privacy</h2>

                        <p className="profile-privacy-text">
                            Your personal account details are
                            available only through authorized access.
                            Use fictional information for demo profiles.
                        </p>

                    </section>

                </aside>
            </div>
            {avatarModalOpen && (
                <AvatarUploadModal
                    avatarUrl={profile.avatarUrl}
                    fullName={profile.fullName}
                    onClose={() => setAvatarModalOpen(false)}
                    onSuccess={handleAvatarSuccess}
                />
            )}
        </div>
    );
}

function InfoRow({ icon: Icon, label, value }) {
    return (
        <div className="profile-info-row">

            <div className="profile-info-label">
                <div className="profile-info-icon">
                    <Icon />
                </div>

                <span>{label}</span>
            </div>

            <strong>{value || "Not provided"}</strong>

        </div>
    );
}

export default Profile;
