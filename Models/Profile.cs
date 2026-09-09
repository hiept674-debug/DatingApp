namespace DatingAppAPI.Models;

public class Profile
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

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public User? User { get; set; }

    public ICollection<Photo> Photos { get; set; } = new List<Photo>();
}