using DatingAppAPI.DTOs;
using DatingAppAPI.DTOs.Auth;
using DatingAppAPI.DTOs.Common;
using DatingAppAPI.DTOs.Users;

namespace DatingAppAPI.Services;

public interface IUserService
{
    Task<PagedResult<UserResponseDto>> GetAllAsync(
        UserQueryParameters queryParams);

    Task<UserResponseDto?> GetByIdAsync(int id);

    Task<(bool IsSuccess, string Message, UserResponseDto? Data)>
        RegisterAsync(UserRegisterDto request);

    Task<(bool IsSuccess, string Message)>
        DeleteAsync(int id);

    // PROFILE
    Task<UserProfileDto?> GetMyProfileAsync(int userId);

    Task<UserProfileDto?> UpdateMyProfileAsync(
        int userId,
        UpdateProfileDto request);
}