export default function filterSampleProfiles(
    profiles,
    filters
) {
    const ids = filters.interestIds.map(Number);

    return profiles.filter(profile => {
        const activeInterests = profile.interests
            .filter(item => item.isActive);

        const profileIds = activeInterests
            .map(item => Number(item.id));

        const matchesCategory =
            !filters.category ||
            activeInterests.some(
                item => item.category === filters.category
            );

        const matchesInterests =
            ids.length === 0 ||
            (
                filters.matchAllInterests
                    ? ids.every(id => profileIds.includes(id))
                    : ids.some(id => profileIds.includes(id))
            );

        return matchesCategory && matchesInterests;
    });
}