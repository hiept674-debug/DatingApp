namespace DatingAppAPI.DTOs.Discovery;

public class DiscoveryUserDto
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string Bio { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }
}