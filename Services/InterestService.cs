using DatingAppAPI.DTOs.Interests;
using DatingAppAPI.Models;
using DatingAppAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace DatingAppAPI.Services;

public class InterestService : IInterestService
{
    private readonly IInterestRepository _repository;

    private const int MaxInterests = 10;

    public InterestService(
        IInterestRepository repository)
    {
        _repository = repository;
    }

    // Lấy danh mục sở thích
    public async Task<List<InterestDto>> GetAllAsync(
        bool includeInactive = false)
    {
        var interests = await _repository
            .GetAllAsync(includeInactive);

        return interests
            .Select(MapToDto)
            .ToList();
    }

    // Lấy sở thích của tài khoản hiện tại
    public async Task<List<InterestDto>?>
        GetMyInterestsAsync(int userId)
    {
        if (!await _repository
            .IsActiveUserAsync(userId))
        {
            return null;
        }

        var interests = await _repository
            .GetUserInterestsAsync(userId);

        return interests
            .Select(MapToDto)
            .ToList();
    }

    // Cập nhật sở thích cá nhân
    public async Task<(bool IsSuccess, string Message)>
        UpdateMyInterestsAsync(
            int userId,
            UpdateUserInterestsDto request)
    {
        if (request.InterestIds == null)
        {
            return (false,
                "Danh sách sở thích không hợp lệ.");
        }

        var ids = request.InterestIds
            .Distinct()
            .ToList();

        if (ids.Count != request.InterestIds.Count)
        {
            return (false,
                "Danh sách sở thích bị trùng lặp.");
        }

        if (ids.Count > MaxInterests)
        {
            return (false,
                "Chỉ được chọn tối đa 10 sở thích.");
        }

        if (ids.Any(id => id <= 0))
        {
            return (false,
                "InterestId không hợp lệ.");
        }

        try
        {
            var success = await _repository
                .ReplaceUserInterestsAsync(
                    userId,
                    ids);

            if (!success)
            {
                return (false,
                    "Tài khoản hoặc sở thích không hợp lệ.");
            }

            return (true,
                "Cập nhật sở thích thành công.");
        }
        catch (SqlException ex)
            when (ex.Number == 1205)
        {
            return (false,
                "Dữ liệu đang được cập nhật. Vui lòng thử lại.");
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is SqlException sql &&
                  sql.Number is 1205 or 2601 or 2627)
        {
            return (false,
                "Dữ liệu đang được cập nhật. Vui lòng thử lại.");
        }
    }

    // Admin tạo sở thích
    public async Task<(bool IsSuccess,
                       string Message,
                       InterestDto? Data)>
        CreateAsync(CreateInterestDto request)
    {
        var name = request.Name.Trim();
        var category = request.Category.Trim();

        if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(category))
        {
            return (false,
                "Tên và danh mục không được để trống.",
                null);
        }

        if (await _repository
            .ExistsByNameAsync(name))
        {
            return (false,
                "Sở thích đã tồn tại.",
                null);
        }

        var interest = new Interest
        {
            Name = name,
            Category = category,
            Icon = request.Icon?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await _repository.AddAsync(interest);

            await _repository.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is SqlException sql &&
                  sql.Number is 2601 or 2627)
        {
            return (false,
                "Sở thích đã tồn tại.",
                null);
        }

        return (true,
            "Tạo sở thích thành công.",
            MapToDto(interest));
    }

    // Admin chỉnh sửa sở thích
    public async Task<(bool IsSuccess, string Message)>
        UpdateAsync(
            int id,
            UpdateInterestDto request)
    {
        var interest = await _repository
            .GetByIdAsync(id);

        if (interest == null)
        {
            return (false,
                "Không tìm thấy sở thích.");
        }

        var name = request.Name.Trim();
        var category = request.Category.Trim();

        if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(category))
        {
            return (false,
                "Tên và danh mục không được để trống.");
        }

        if (await _repository
            .ExistsByNameAsync(name, id))
        {
            return (false,
                "Tên sở thích đã tồn tại.");
        }

        interest.Name = name;
        interest.Category = category;
        interest.Icon = request.Icon?.Trim();
        interest.IsActive = request.IsActive;

        try
        {
            await _repository.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is SqlException sql &&
                  sql.Number is 2601 or 2627)
        {
            return (false,
                "Tên sở thích đã tồn tại.");
        }

        return (true,
            "Cập nhật sở thích thành công.");
    }

    // Admin vô hiệu hóa sở thích
    public async Task<(bool IsSuccess, string Message)>
        DeleteAsync(int id)
    {
        var interest = await _repository
            .GetByIdAsync(id);

        if (interest == null)
        {
            return (false,
                "Không tìm thấy sở thích.");
        }

        interest.IsActive = false;

        await _repository.SaveChangesAsync();

        return (true,
            "Đã vô hiệu hóa sở thích.");
    }

    // Mapping Entity sang DTO
    private static InterestDto MapToDto(
        Interest interest)
    {
        return new InterestDto
        {
            Id = interest.Id,
            Name = interest.Name,
            Category = interest.Category,
            Icon = interest.Icon,
            IsActive = interest.IsActive
        };
    }
}