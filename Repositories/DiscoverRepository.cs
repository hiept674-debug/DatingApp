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

        // Search
        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.Trim();

            query = query.Where(u =>
                u.FullName.Contains(search));
        }

        // Sorting
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

        // Count
        var totalCount = await query.CountAsync();

        // Pagination
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