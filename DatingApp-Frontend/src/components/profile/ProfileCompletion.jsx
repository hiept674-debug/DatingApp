import {
    FaUser,
    FaImage,
    FaFileAlt,
    FaStar,
    FaCheckCircle,
    FaRegCircle
} from "react-icons/fa";

import "./ProfileCompletion.css";

export default function ProfileCompletion({
    profile
}) {
    const details = profile?.completionDetails;

    const percentage = Math.min(
        100,
        Math.max(
            0,
            Number(profile?.profileCompletion) || 0
        )
    );

    const basicScore = details
        ? [
            details.hasFullName,
            details.hasEmail,
            details.hasPhone,
            details.hasDateOfBirth,
            details.hasGender,
            details.hasCity
        ].filter(Boolean).length * 10
        : 0;

    const sections = [
        {
            id: "basic",
            name: "Basic information",
            icon: FaUser,
            score: basicScore,
            maxScore: 60,
            completed: details?.hasBasicInformation
        },
        {
            id: "avatar",
            name: "Profile picture",
            icon: FaImage,
            score: details?.hasAvatar ? 15 : 0,
            maxScore: 15,
            completed: details?.hasAvatar
        },
        {
            id: "bio",
            name: "Biography",
            icon: FaFileAlt,
            score: details?.hasBio ? 10 : 0,
            maxScore: 10,
            completed: details?.hasBio
        },
        {
            id: "interests",
            name: "Personal interests",
            icon: FaStar,
            score: details?.hasInterests ? 15 : 0,
            maxScore: 15,
            completed: details?.hasInterests
        }
    ];

    const completedCount = sections.filter(
        item => item.completed
    ).length;

    return (
        <section className="pc-panel">

            <div className="pc-heading">
                <h2>Profile Completion</h2>

                <p>Your profile overview</p>
            </div>

            <div
                className="pc-circle"
                style={{
                    "--pc-progress": `${percentage}%`
                }}
                role="progressbar"
                aria-label="Profile completion"
                aria-valuenow={percentage}
                aria-valuemin={0}
                aria-valuemax={100}
            >
                <div className="pc-circle-inner">
                    <strong>{percentage}%</strong>

                    <span>Completed</span>
                </div>
            </div>

            <div className="pc-summary">

                <h3>
                    {percentage === 100
                        ? "Profile complete"
                        : "Profile information"}
                </h3>

                {details && (
                    <p>
                        {completedCount}/4 sections completed
                    </p>
                )}

            </div>

            {details && (
                <div className="pc-checklist">

                    {sections.map(item => {
                        const Icon = item.icon;

                        return (
                            <div
                                key={item.id}
                                className="pc-item"
                            >
                                <div className="pc-item-icon">
                                    <Icon />
                                </div>

                                <div className="pc-item-info">

                                    <strong>
                                        {item.name}
                                    </strong>

                                    <span>
                                        {item.completed
                                            ? "Completed"
                                            : "Not complete"}
                                    </span>

                                </div>

                                <span className="pc-item-score">
                                    {item.score}/{item.maxScore}
                                </span>

                                {item.completed ? (
                                    <FaCheckCircle
                                        className="pc-done"
                                    />
                                ) : (
                                    <FaRegCircle
                                        className="pc-pending"
                                    />
                                )}

                            </div>
                        );
                    })}

                </div>
            )}

            <div className="pc-note">
                Additional profile information is optional.
            </div>

        </section>
    );
}