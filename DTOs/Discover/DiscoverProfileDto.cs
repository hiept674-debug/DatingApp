namespace DatingAppAPI.DTOs.Discover;

public class DiscoverProfileDto
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Bio { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }

    public DateTime CreatedAt { get; set; }
}