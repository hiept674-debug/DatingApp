using DatingAppAPI.Models;
using DatingAppAPI.Data;
using DatingAppAPI.DTOs.Common;
using Microsoft.EntityFrameworkCore;

namespace DatingAppAPI.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<User>> GetAllAsync(
        UserQueryParameters queryParams)
    {
        var pageNumber = Math.Max(1, queryParams.PageNumber);
        var pageSize = Math.Clamp(queryParams.PageSize, 1, 100);

        var query = _db.Users.AsNoTracking().AsQueryable();

        // 1. SEARCH
        if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
        {
            var term = queryParams.SearchTerm.Trim().ToLower();

            query = query.Where(u =>
                u.FullName.ToLower().Contains(term) ||
                u.Email.ToLower().Contains(term));
        }

        // 2. FILTER ROLE
        if (!string.IsNullOrWhiteSpace(queryParams.Role))
        {
            query = query.Where(u => u.Role == queryParams.Role);
        }

        // 3. SORTING
        query = queryParams.SortBy?.ToLower() switch
        {
            "email" => queryParams.IsDescending
                ? query.OrderByDescending(u => u.Email)
                : query.OrderBy(u => u.Email),

            "createdat" => queryParams.IsDescending
                ? query.OrderByDescending(u => u.CreatedAt)
                : query.OrderBy(u => u.CreatedAt),

            _ => queryParams.IsDescending
                ? query.OrderByDescending(u => u.FullName)
                : query.OrderBy(u => u.FullName)
        };

        // 4. COUNT
        var totalCount = await query.CountAsync();

        // 5. PAGINATION
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<User>(
            items,
            totalCount,
            pageNumber,
            pageSize
        );
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByUsernameOrEmailAsync(
        string username, string email)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddAsync(User user)
    {
        await _db.Users.AddAsync(user);
    }

    public void Update(User user)
    {
        _db.Users.Update(user);
    }

    public void Delete(User user)
    {
        _db.Users.Remove(user);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _db.SaveChangesAsync() > 0;
    }
}