using DatingAppAPI.DTOs.Discover;

namespace DatingAppAPI.Services.Discover;

public record SampleInterest(
    int Id,
    string Name,
    string Category,
    bool IsActive
);

public record SampleProfile(
    int Id,
    string DisplayName,
    string Bio,
    DateTime CreatedAt,
    IReadOnlyList<SampleInterest> Interests
);

public static class DemoProfileFilter
{
    public static IEnumerable<SampleProfile> Apply(
        IEnumerable<SampleProfile> profiles,
        DiscoverQueryParameters filter)
    {
        var query = profiles;

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var keyword = filter.Search.Trim();

            query = query.Where(profile =>
                profile.DisplayName.Contains(
                    keyword,
                    StringComparison.OrdinalIgnoreCase)
                ||
                profile.Bio.Contains(
                    keyword,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(filter.Category))
        {
            var category = filter.Category.Trim();

            query = query.Where(profile =>
                profile.Interests.Any(interest =>
                    interest.IsActive &&
                    interest.Category.Equals(
                        category,
                        StringComparison.OrdinalIgnoreCase)));
        }

        var interestIds = filter.InterestIds
            .Where(id => id > 0)
            .Distinct()
            .ToArray();

        if (interestIds.Length > 0)
        {
            query = query.Where(profile =>
            {
                var activeIds = profile.Interests
                    .Where(i => i.IsActive)
                    .Select(i => i.Id)
                    .ToHashSet();

                return filter.MatchAllInterests
                    ? interestIds.All(activeIds.Contains)
                    : interestIds.Any(activeIds.Contains);
            });
        }

        return filter.SortBy?.ToLowerInvariant() switch
        {
            "oldest" => query
                .OrderBy(p => p.CreatedAt)
                .ThenBy(p => p.Id),

            "name_asc" => query
                .OrderBy(p => p.DisplayName)
                .ThenBy(p => p.Id),

            "name_desc" => query
                .OrderByDescending(p => p.DisplayName)
                .ThenBy(p => p.Id),

            _ => query
                .OrderByDescending(p => p.CreatedAt)
                .ThenBy(p => p.Id)
        };
    }
}