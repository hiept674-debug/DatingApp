
import { useEffect, useState } from "react";
import userService from "../services/userService";

import "./Avatar.css";

function Avatar({
    avatarUrl,
    fullName,
    size = 48,
    className = "",
}) {
    const [image, setImage] = useState(null);

    useEffect(() => {
        let objectUrl = null;
        let active = true;

        const controller = new AbortController();

        setImage(null);

        if (!avatarUrl) {
            return () => controller.abort();
        }

        const loadAvatar = async () => {
            try {
                const blob = await userService.getAvatarBlob(
                    avatarUrl,
                    controller.signal
                );

                if (!active || !blob) return;

                objectUrl = URL.createObjectURL(blob);

                setImage({
                    source: avatarUrl,
                    url: objectUrl,
                });
            } catch (error) {
                if (active) {
                    setImage(null);
                }
            }
        };

        loadAvatar();

        return () => {
            active = false;

            controller.abort();

            if (objectUrl) {
                URL.revokeObjectURL(objectUrl);
            }
        };
    }, [avatarUrl]);

    const initial =
        fullName?.trim().charAt(0).toUpperCase() || "U";

    const imageUrl =
        image?.source === avatarUrl
            ? image.url
            : null;

    return (
        <div
            className={`app-avatar ${className}`}
            style={{
                width: size,
                height: size,
                fontSize: size * 0.38,
            }}
        >
            {imageUrl ? (
                <img
                    src={imageUrl}
                    alt="Profile avatar"
                    onError={() => setImage(null)}
                />
            ) : (
                <span>{initial}</span>
            )}
        </div>
    );
}

export default Avatar;
