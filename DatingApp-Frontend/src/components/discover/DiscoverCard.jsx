import {
    FaArrowRight,
    FaCalendarAlt,
    FaUser
} from "react-icons/fa";

import Avatar from "../Avatar";

export default function DiscoverCard({
    profile,
    onViewProfile
}) {
    const memberSince = profile.createdAt
        ? new Date(profile.createdAt).toLocaleDateString("vi-VN")
        : "Chưa cập nhật";

    return (
        <article className="discover-card">

            <div className="discover-card-cover">
                <Avatar
                    avatarUrl={profile.avatarUrl}
                    fullName={profile.fullName}
                    size={120}
                    className="discover-avatar-v2"
                />
            </div>

            <div className="discover-card-body">

                <h3>{profile.fullName}</h3>

                <div className="discover-card-meta">
                    <FaUser />
                    <span>Profile</span>
                </div>

                <p className="discover-card-bio">
                    {profile.bio || "Chưa có thông tin giới thiệu."}
                </p>

                <div className="discover-card-details">
                    <FaCalendarAlt />
                    <span>Tham gia: {memberSince}</span>
                </div>

                <button
                    className="discover-view-btn"
                    onClick={() => onViewProfile(profile.id)}
                >
                    View profile
                    <FaArrowRight />
                </button>

            </div>

        </article>
    );
}