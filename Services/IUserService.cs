using DatingAppAPI.DTOs;
using DatingAppAPI.DTOs.Auth;
using DatingAppAPI.DTOs.Common;
namespace DatingAppAPI.Services;

public interface IUserService
{
    Task<PagedResult<UserResponseDto>> GetAllAsync(UserQueryParameters queryParams);

    Task<UserResponseDto?> GetByIdAsync(int id);

    Task<(bool IsSuccess, string Message, UserResponseDto? Data)>
        RegisterAsync(UserRegisterDto request);

    Task<(bool IsSuccess, string Message)>
        UpdateAsync(int id, UserRegisterDto request);

    Task<(bool IsSuccess, string Message)>
        DeleteAsync(int id);
}   