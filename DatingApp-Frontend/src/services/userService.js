
import api from "./api";

const userService = {
    // Lấy thông tin hồ sơ
    getMyProfile: async () => {
        const response = await api.get("/users/me");

        return response.data;
    },

    // Cập nhật hồ sơ
    updateMyProfile: async (data) => {
        const response = await api.put("/users/me", data);

        return response.data;
    },

    // Upload avatar
    uploadAvatar: async (file, onProgress) => {
        const formData = new FormData();

        formData.append("File", file);

        const response = await api.post(
            "/users/me/avatar",
            formData,
            {
                headers: {
                    "Content-Type": "multipart/form-data",
                },

                onUploadProgress: (event) => {
                    if (event.total && onProgress) {
                        const percent = Math.round(
                            (event.loaded * 100) / event.total
                        );

                        onProgress(percent);
                    }
                },
            }
        );

        return response.data;
    },

    // Xóa avatar
    deleteAvatar: async () => {
        const response = await api.delete(
            "/users/me/avatar"
        );

        return response.data;
    },

    // Đọc ảnh có xác thực JWT
    getAvatarBlob: async (avatarUrl, signal) => {
        if (!avatarUrl) return null;

        const match = avatarUrl.match(
            /^\/api\/files\/(\d+)$/
        );

        if (!match) {
            throw new Error("Invalid avatar URL.");
        }

        const response = await api.get(
            `/files/${match[1]}`,
            {
                responseType: "blob",
                signal,
            }
        );

        return response.data;
    },
};

export default userService;
