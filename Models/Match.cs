namespace DatingAppAPI.Models;

using System.ComponentModel.DataAnnotations;

public class Match
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int User1Id { get; set; }

    [Required]
    public int User2Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User1 { get; set; } = null!;

    public User User2 { get; set; } = null!;

    public Conversation? Conversation { get; set; }
}