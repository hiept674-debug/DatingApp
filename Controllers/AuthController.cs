
using DatingAppAPI.DTOs.Auth;
using DatingAppAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DatingAppAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
        {
            var result = await _authService.RegisterAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        if (result == null)
        {
            return Unauthorized(new
            {
                message = "Email hoặc mật khẩu không chính xác."
            });
        }

        return Ok(result);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new
        {
            message = "Đăng xuất thành công."
        });
    }
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
    [FromBody] ForgotPasswordDto dto)
    {
        await _authService.ForgotPasswordAsync(dto);

        return Ok(new
        {
            message = "Nếu email đã được đăng ký, " +
                      "mã OTP sẽ được gửi đến hộp thư."
        });
    }

    [HttpPost("verify-reset-code")]
    public async Task<IActionResult> VerifyResetCode(
    [FromBody] VerifyResetCodeDto dto)
    {
        var result = await _authService.VerifyResetCodeAsync(dto);

        if (result == null)
        {
            return BadRequest(new
            {
                message = "Mã OTP không hợp lệ hoặc đã hết hạn."
            });
        }

        return Ok(result);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordDto dto)
    {
        var success = await _authService.ResetPasswordAsync(dto);

        if (!success)
        {
            return BadRequest(new
            {
                message = "Yêu cầu đổi mật khẩu không hợp lệ hoặc đã hết hạn."
            });
        }

        return Ok(new
        {
            message = "Đổi mật khẩu thành công"
        });
    }
}
