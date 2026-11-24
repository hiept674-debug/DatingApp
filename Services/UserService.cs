using DatingAppAPI.DTOs;
using DatingAppAPI.DTOs.Auth;
using DatingAppAPI.Models;
using DatingAppAPI.DTOs.Common;
namespace DatingAppAPI.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;

    public UserService(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<PagedResult<UserResponseDto>> GetAllAsync(UserQueryParameters queryParams)
    {
        var pagedUsers = await _userRepo.GetAllAsync(queryParams);

        // Map danh sách Entity sang DTO
        var mappedItems = pagedUsers.Items.Select(MapToResponseDto).ToList();

        // Trả về PagedResult chứa DTO
        return new PagedResult<UserResponseDto>(
            mappedItems,
            pagedUsers.TotalCount,
            pagedUsers.PageNumber,
            pagedUsers.PageSize
        );
    }

    public async Task<UserResponseDto?> GetByIdAsync(int id)
    {
        var user = await _userRepo.GetByIdAsync(id);

        return user == null ? null : MapToResponseDto(user);
    }

    public async Task<(bool IsSuccess, string Message, UserResponseDto? Data)> RegisterAsync(
        UserRegisterDto request)
    {
        var existingUser = await _userRepo.GetByUsernameOrEmailAsync(
            "",
            request.Email
        );

        if (existingUser != null)
        {
            return (false, "Email đã được sử dụng.", null);
        }

        var newUser = new User
        {
            Email = request.Email,
            Phone = request.Phone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FullName = request.FullName,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Bio = request.Bio,
            City = request.City,
            AvatarUrl = request.AvatarUrl,
            Role = "User",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepo.AddAsync(newUser);
        await _userRepo.SaveChangesAsync();

        return (
            true,
            "Đăng ký thành công",
            MapToResponseDto(newUser)
        );
    }

    public async Task<(bool IsSuccess, string Message)> UpdateAsync(
        int id,
        UserRegisterDto request)
    {
        var user = await _userRepo.GetByIdAsync(id);

        if (user == null)
        {
            return (
                false,
                $"Không tìm thấy người dùng có Id = {id}"
            );
        }

        if (!string.IsNullOrEmpty(request.Email))
            user.Email = request.Email;

        if (!string.IsNullOrEmpty(request.Phone))
            user.Phone = request.Phone;

        if (!string.IsNullOrEmpty(request.Password))
            user.PasswordHash = request.Password;

        if (!string.IsNullOrEmpty(request.FullName))
            user.FullName = request.FullName;

        if (request.DateOfBirth != default)
            user.DateOfBirth = request.DateOfBirth;

        if (!string.IsNullOrEmpty(request.Gender))
            user.Gender = request.Gender;

        if (!string.IsNullOrEmpty(request.Bio))
            user.Bio = request.Bio;

        if (!string.IsNullOrEmpty(request.City))
            user.City = request.City;

        if (request.AvatarUrl != null)
            user.AvatarUrl = request.AvatarUrl;

        if (!string.IsNullOrEmpty(request.Role))
            user.Role = request.Role;

        user.UpdatedAt = DateTime.UtcNow;

        _userRepo.Update(user);
        await _userRepo.SaveChangesAsync();

        return (
            true,
            $"Cập nhật thành công người dùng Id = {id}"
        );
    }

    public async Task<(bool IsSuccess, string Message)> DeleteAsync(int id)
    {
        var user = await _userRepo.GetByIdAsync(id);

        if (user == null)
        {
            return (
                false,
                $"Không tìm thấy người dùng có Id = {id}"
            );
        }

        _userRepo.Delete(user);
        await _userRepo.SaveChangesAsync();

        return (
            true,
            $"Đã xóa người dùng Id = {id}"
        );
    }

    private static UserResponseDto MapToResponseDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            Phone = user.Phone,
            FullName = user.FullName,
            DateOfBirth = user.DateOfBirth,
            Gender = user.Gender,
            Bio = user.Bio,
            City = user.City,
            AvatarUrl = user.AvatarUrl,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}