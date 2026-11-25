using DatingAppAPI.Data;
using DatingAppAPI.Models;
using DatingAppAPI.DTOs.Discover;
using Microsoft.EntityFrameworkCore;

namespace DatingAppAPI.Repositories;

public class DiscoverRepository : IDiscoverRepository
{
    private readonly AppDbContext _context;

    public DiscoverRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<User> Items, int TotalCount)>
        GetProfilesAsync(DiscoverQueryParameters parameters)
    {
        IQueryable<User> query = _context.Users
            .AsNoTracking()
            .Where(u => u.Role == "User" && u.IsActive);

        // 1. Search
        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.Trim();

            query = query.Where(u =>
                u.FullName.Contains(search));
        }

        // 2. Category Filter
        if (!string.IsNullOrWhiteSpace(parameters.Category))
        {
            var category = parameters.Category.Trim();

            query = query.Where(u =>
                u.UserInterests.Any(ui =>
                    ui.Interest.IsActive &&
                    ui.Interest.Category == category));
        }

        // 3. Interest Filter
        var interestIds = parameters.InterestIds
            .Distinct()
            .ToArray();

        if (interestIds.Length > 0)
        {
            if (parameters.MatchAllInterests)
            {
                // Phải có tất cả sở thích đã chọn
                query = query.Where(u =>
                    u.UserInterests.Count(ui =>
                        ui.Interest.IsActive &&
                        interestIds.Contains(ui.InterestId))
                    == interestIds.Length);
            }
            else
            {
                // Có ít nhất một sở thích đã chọn
                query = query.Where(u =>
                    u.UserInterests.Any(ui =>
                        ui.Interest.IsActive &&
                        interestIds.Contains(ui.InterestId)));
            }
        }

        // 4. Sorting
        query = parameters.SortBy?.ToLowerInvariant() switch
        {
            "name_asc" => query
                .OrderBy(u => u.FullName)
                .ThenBy(u => u.Id),

            "name_desc" => query
                .OrderByDescending(u => u.FullName)
                .ThenBy(u => u.Id),

            "oldest" => query
                .OrderBy(u => u.CreatedAt)
                .ThenBy(u => u.Id),

            _ => query
                .OrderByDescending(u => u.CreatedAt)
                .ThenByDescending(u => u.Id)
        };

        // 5. Count after filtering
        var totalCount = await query.CountAsync();

        // 6. Pagination
        var pageNumber = Math.Clamp(
            parameters.PageNumber, 1, 100000);

        var pageSize = Math.Clamp(
            parameters.PageSize, 1, 24);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u =>
                u.Id == id &&
                u.Role == "User" &&
                u.IsActive);
    }
}