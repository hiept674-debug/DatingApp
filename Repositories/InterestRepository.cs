using System.Data;
using DatingAppAPI.Data;
using DatingAppAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingAppAPI.Repositories;

public class InterestRepository : IInterestRepository
{
    private readonly AppDbContext _context;

    public InterestRepository(AppDbContext context)
    {
        _context = context;
    }

    // Lấy danh sách sở thích
    public async Task<List<Interest>> GetAllAsync(
        bool includeInactive = false)
    {
        IQueryable<Interest> query =
            _context.Interests.AsNoTracking();

        if (!includeInactive)
        {
            query = query.Where(i => i.IsActive);
        }

        return await query
            .OrderBy(i => i.Category)
            .ThenBy(i => i.Name)
            .ToListAsync();
    }

    // Lấy sở thích theo ID
    public async Task<Interest?> GetByIdAsync(int id)
    {
        return await _context.Interests
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    // Kiểm tra trùng tên
    public async Task<bool> ExistsByNameAsync(
        string name,
        int? excludeId = null)
    {
        var normalizedName =
            name.Trim().ToLowerInvariant();

        return await _context.Interests.AnyAsync(i =>
            i.Name.ToLower() == normalizedName &&
            (!excludeId.HasValue ||
             i.Id != excludeId.Value));
    }

    // Kiểm tra tài khoản còn hoạt động
    public async Task<bool> IsActiveUserAsync(
        int userId)
    {
        return await _context.Users.AnyAsync(u =>
            u.Id == userId &&
            u.IsActive);
    }

    // Lấy sở thích cá nhân
    public async Task<List<Interest>>
        GetUserInterestsAsync(int userId)
    {
        return await _context.UserInterests
            .AsNoTracking()
            .Where(ui =>
                ui.UserId == userId &&
                ui.Interest.IsActive)
            .Select(ui => ui.Interest)
            .OrderBy(i => i.Category)
            .ThenBy(i => i.Name)
            .ToListAsync();
    }

    // Cập nhật toàn bộ danh sách sở thích
    public async Task<bool> ReplaceUserInterestsAsync(
        int userId,
        List<int> interestIds)
    {
        await using var transaction =
            await _context.Database
                .BeginTransactionAsync(
                    IsolationLevel.Serializable);

        var activeUser = await _context.Users
            .AnyAsync(u =>
                u.Id == userId &&
                u.IsActive);

        if (!activeUser)
        {
            return false;
        }

        var validCount = await _context.Interests
            .CountAsync(i =>
                interestIds.Contains(i.Id) &&
                i.IsActive);

        if (validCount != interestIds.Count)
        {
            return false;
        }

        var currentInterests =
            await _context.UserInterests
                .Where(ui => ui.UserId == userId)
                .ToListAsync();

        // Chỉ xóa những sở thích bị bỏ chọn
        var removedInterests = currentInterests
            .Where(ui =>
                !interestIds.Contains(ui.InterestId))
            .ToList();

        _context.UserInterests
            .RemoveRange(removedInterests);

        var existingIds = currentInterests
            .Select(ui => ui.InterestId)
            .ToHashSet();

        // Chỉ thêm những sở thích mới
        var newInterests = interestIds
            .Where(id => !existingIds.Contains(id))
            .Select(id => new UserInterest
            {
                UserId = userId,
                InterestId = id,
                CreatedAt = DateTime.UtcNow
            })
            .ToList();

        await _context.UserInterests
            .AddRangeAsync(newInterests);

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        return true;
    }

    public async Task AddAsync(Interest interest)
    {
        await _context.Interests.AddAsync(interest);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}