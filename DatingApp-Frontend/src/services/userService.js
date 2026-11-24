
import api from "./api";

const userService = {
    getMyProfile: async () => {
        const response = await api.get("/users/me");

        return response.data;
    },

    updateMyProfile: async (data) => {
        const response = await api.put("/users/me", data);

        return response.data;
    },
};

export default userService;
