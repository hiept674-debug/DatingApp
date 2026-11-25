import api from "./api";

const interestService = {
    // Lấy toàn bộ danh mục đang hoạt động
    getAll: async (signal) => {
        const response = await api.get(
            "/interests",
            { signal }
        );

        return response.data;
    },

    // Lấy sở thích của tài khoản hiện tại
    getMyInterests: async (signal) => {
        const response = await api.get(
            "/interests/me",
            { signal }
        );

        return response.data;
    },

    // Cập nhật sở thích cá nhân
    updateMyInterests: async (interestIds) => {
        const response = await api.put(
            "/interests/me",
            { interestIds }
        );

        return response.data;
    },

    // Admin: Lấy toàn bộ danh mục
    getAllForAdmin: async () => {
        const response = await api.get(
            "/interests/admin"
        );

        return response.data;
    },

    // Admin: Tạo sở thích
    create: async (data) => {
        const response = await api.post(
            "/interests",
            data
        );

        return response.data;
    },

    // Admin: Chỉnh sửa sở thích
    update: async (id, data) => {
        const response = await api.put(
            `/interests/${id}`,
            data
        );

        return response.data;
    },

    // Admin: Vô hiệu hóa sở thích
    remove: async (id) => {
        const response = await api.delete(
            `/interests/${id}`
        );

        return response.data;
    }
};

export default interestService;