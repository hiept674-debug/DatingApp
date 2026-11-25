import api from "./api";

const discoverService = {
    getProfiles: async (
        {
            search = "",
            sortBy = "newest",
            pageNumber = 1,
            pageSize = 6
        } = {},
        signal
    ) => {
        const response = await api.get("/discover", {
            params: {
                search,
                sortBy,
                pageNumber,
                pageSize
            },
            signal
        });

        return response.data;
    },

    getProfileById: async (id, signal) => {
        const response = await api.get(
            `/discover/${id}`,
            { signal }
        );

        return response.data;
    }
};

export default discoverService;