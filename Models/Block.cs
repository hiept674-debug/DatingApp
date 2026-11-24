namespace DatingAppAPI.Models;

using System.ComponentModel.DataAnnotations;

public class Block
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BlockerId { get; set; }

    [Required]
    public int BlockedUserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User Blocker { get; set; } = null!;

    public User BlockedUser { get; set; } = null!;
}