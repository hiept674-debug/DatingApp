namespace DatingAppAPI.DTOs.Auth;

public class AuthResponseDto
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;
}