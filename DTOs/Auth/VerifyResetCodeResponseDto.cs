
namespace DatingAppAPI.DTOs.Auth;

public class VerifyResetCodeResponseDto
{
	public string Message { get; set; } = string.Empty;

	public string ResetToken { get; set; } = string.Empty;
}
