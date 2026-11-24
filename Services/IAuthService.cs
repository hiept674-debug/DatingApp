using DatingAppAPI.DTOs.Auth;

namespace DatingAppAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task ForgotPasswordAsync(ForgotPasswordDto dto);

        Task<VerifyResetCodeResponseDto?> VerifyResetCodeAsync(
            VerifyResetCodeDto dto);

        Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
    }
}
