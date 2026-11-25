using System.ComponentModel.DataAnnotations;

namespace DatingAppAPI.Models;

public class DemoProfile
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Bio { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Interest { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Occupation { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? AvatarUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;

    public ICollection<DemoProfileInterest> DemoProfileInterests
    { get; set; } = new List<DemoProfileInterest>();
}