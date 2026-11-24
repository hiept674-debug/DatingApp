namespace DatingAppAPI.Models;

using System.ComponentModel.DataAnnotations;

public class Like
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int FromUserId { get; set; }

    [Required]
    public int ToUserId { get; set; }

    [Required]
    public bool IsLike { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User FromUser { get; set; } = null!;

    public User ToUser { get; set; } = null!;
}