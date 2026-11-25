namespace DatingAppAPI.Models;

public class UserInterest
{
    public int UserId { get; set; }

    public int InterestId { get; set; }

    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;

    public User User { get; set; } = null!;

    public Interest Interest { get; set; } = null!;
}