namespace DatingAppAPI.DTOs;

public class ProfileDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string Bio { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string? Job { get; set; }

    public string? Education { get; set; }

    public string? Interests { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public ICollection<PhotoDto> Photos { get; set; } = new List<PhotoDto>();
}