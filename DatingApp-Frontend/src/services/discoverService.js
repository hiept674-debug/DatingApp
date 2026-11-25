import api from "./api";

const discoverService = {
    getProfiles: async (
        {
            search = "",
            sortBy = "newest",
            pageNumber = 1,
            pageSize = 6,
            category = "",
            interestIds = [],
            matchAllInterests = false
        } = {},
        signal
    ) => {
        const params = new URLSearchParams();

        params.append("search", search);
        params.append("sortBy", sortBy);
        params.append("pageNumber", pageNumber);
        params.append("pageSize", pageSize);
        params.append("category", category);
        params.append(
            "matchAllInterests",
            matchAllInterests
        );

        interestIds.forEach(id => {
            params.append("interestIds", id);
        });

        const response = await api.get("/discover", {
            params,
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