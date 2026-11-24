namespace DatingAppAPI.Models;

using System.ComponentModel.DataAnnotations;

public class Conversation
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int MatchId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Match Match { get; set; } = null!;

    public ICollection<Message> Messages { get; set; }
        = new List<Message>();
}