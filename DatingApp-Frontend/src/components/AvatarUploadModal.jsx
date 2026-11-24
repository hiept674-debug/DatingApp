
import { useEffect, useRef, useState } from "react";

import {
    FaTimes,
    FaCloudUploadAlt,
    FaImage,
    FaSpinner,
    FaCheck,
    FaExclamationCircle,
} from "react-icons/fa";

import userService from "../services/userService";
import Avatar from "./Avatar";

import "./AvatarUploadModal.css";

const MAX_FILE_SIZE = 512 * 1024;

const ALLOWED_TYPES = [
    "image/jpeg",
    "image/png",
    "image/webp",
];

function AvatarUploadModal({
    avatarUrl,
    fullName,
    onClose,
    onSuccess,
}) {
    const inputRef = useRef(null);

    const [file, setFile] = useState(null);
    const [preview, setPreview] = useState(null);

    const [uploading, setUploading] = useState(false);
    const [progress, setProgress] = useState(0);

    const [error, setError] = useState("");
    const [dragging, setDragging] = useState(false);

    // Tạo và giải phóng ảnh preview
    useEffect(() => {
        if (!file) {
            setPreview(null);
            return;
        }

        const url = URL.createObjectURL(file);

        setPreview(url);

        return () => URL.revokeObjectURL(url);
    }, [file]);

    // Đóng modal bằng Escape
    useEffect(() => {
        const handleKeyDown = (event) => {
            if (event.key === "Escape" && !uploading) {
                onClose();
            }
        };

        window.addEventListener(
            "keydown",
            handleKeyDown
        );

        return () => {
            window.removeEventListener(
                "keydown",
                handleKeyDown
            );
        };
    }, [uploading, onClose]);

    const selectFile = (selectedFile) => {
        if (!selectedFile || uploading) return;

        setError("");
        setProgress(0);

        if (!ALLOWED_TYPES.includes(selectedFile.type)) {
            setFile(null);

            setError(
                "Only JPG, PNG and WEBP images are allowed."
            );

            return;
        }

        if (selectedFile.size === 0) {
            setFile(null);
            setError("The selected file is empty.");
            return;
        }

        if (selectedFile.size > MAX_FILE_SIZE) {
            setFile(null);

            setError(
                "Image size must not exceed 512 KB."
            );

            return;
        }

        setFile(selectedFile);
    };

    const handleFileChange = (event) => {
        selectFile(event.target.files?.[0]);

        event.target.value = "";
    };

    const handleDrop = (event) => {
        event.preventDefault();

        setDragging(false);

        selectFile(event.dataTransfer.files?.[0]);
    };

    const handleUpload = async () => {
        if (!file || uploading) return;

        try {
            setUploading(true);
            setError("");
            setProgress(0);

            const result =
                await userService.uploadAvatar(
                    file,
                    setProgress
                );

            setProgress(100);

            onSuccess(result);
        } catch (err) {
            const data = err.response?.data;

            setError(
                typeof data === "string"
                    ? data
                    : data?.message ||
                    "Unable to upload avatar."
            );
        } finally {
            setUploading(false);
        }
    };

    return (
        <div
            className="avatar-modal-overlay"
            onClick={() => {
                if (!uploading) onClose();
            }}
        >
            <div
                className="avatar-modal"
                role="dialog"
                aria-modal="true"
                aria-labelledby="avatar-modal-title"
                onClick={(event) => event.stopPropagation()}
            >
                {/* Header */}
                <div className="avatar-modal-header">
                    <div>
                        <h2 id="avatar-modal-title">
                            Change Profile Picture
                        </h2>

                        <p>
                            Select an image for your demo account.
                        </p>
                    </div>

                    <button
                        type="button"
                        className="avatar-modal-close"
                        onClick={onClose}
                        disabled={uploading}
                        aria-label="Close"
                    >
                        <FaTimes />
                    </button>
                </div>

                {/* Preview */}
                <div className="avatar-preview-section">
                    {preview ? (
                        <img
                            className="avatar-image-preview"
                            src={preview}
                            alt="Selected image preview"
                        />
                    ) : (
                        <Avatar
                            avatarUrl={avatarUrl}
                            fullName={fullName}
                            size={120}
                        />
                    )}

                    <span>Image preview</span>
                </div>

                {/* Hidden Input */}
                <input
                    ref={inputRef}
                    type="file"
                    accept="image/jpeg,image/png,image/webp"
                    onChange={handleFileChange}
                    disabled={uploading}
                    hidden
                />

                {/* Drop Zone */}
                <button
                    type="button"
                    className={`avatar-dropzone ${dragging ? "dragging" : ""
                        }`}
                    onClick={() => inputRef.current?.click()}
                    onDragOver={(event) => {
                        event.preventDefault();

                        if (!uploading) {
                            setDragging(true);
                        }
                    }}
                    onDragLeave={() => setDragging(false)}
                    onDrop={handleDrop}
                    disabled={uploading}
                >
                    <FaCloudUploadAlt />

                    <strong>
                        Click to upload or drag and drop
                    </strong>

                    <span>
                        JPG, PNG, WEBP — Max 512 KB
                    </span>
                </button>

                {/* Selected File */}
                {file && (
                    <div className="avatar-selected-file">
                        <FaImage />

                        <div className="avatar-file-info">
                            <strong>{file.name}</strong>

                            <span>
                                {(file.size / 1024).toFixed(1)} KB
                            </span>
                        </div>

                        {!uploading && (
                            <button
                                type="button"
                                onClick={() => {
                                    setFile(null);
                                    setProgress(0);
                                    setError("");
                                }}
                                aria-label="Remove selected file"
                            >
                                <FaTimes />
                            </button>
                        )}
                    </div>
                )}

                {/* Progress */}
                {uploading && (
                    <div className="avatar-upload-progress">
                        <div className="avatar-progress-label">
                            <span>
                                {progress === 100
                                    ? "Processing image..."
                                    : "Uploading..."}
                            </span>

                            <strong>{progress}%</strong>
                        </div>

                        <div
                            className="avatar-progress-track"
                            role="progressbar"
                            aria-valuenow={progress}
                            aria-valuemin={0}
                            aria-valuemax={100}
                            aria-label="Upload progress"
                        >
                            <div
                                className="avatar-progress-fill"
                                style={{
                                    width: `${progress}%`,
                                }}
                            />
                        </div>
                    </div>
                )}

                {/* Error */}
                {error && (
                    <div
                        className="avatar-upload-error"
                        role="alert"
                    >
                        <FaExclamationCircle />

                        <span>{error}</span>
                    </div>
                )}

                {/* Actions */}
                <div className="avatar-modal-actions">
                    <button
                        type="button"
                        className="profile-btn secondary"
                        onClick={onClose}
                        disabled={uploading}
                    >
                        Cancel
                    </button>

                    <button
                        type="button"
                        className="profile-btn primary"
                        onClick={handleUpload}
                        disabled={!file || uploading}
                    >
                        {uploading ? (
                            <FaSpinner className="profile-spinner" />
                        ) : (
                            <FaCheck />
                        )}

                        {uploading
                            ? "Uploading..."
                            : "Save Avatar"}
                    </button>
                </div>
            </div>
        </div>
    );
}

export default AvatarUploadModal;
