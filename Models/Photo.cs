namespace DatingAppAPI.Models;

public class Photo
{
    public int Id { get; set; }

    public int ProfileId { get; set; }

    public string Url { get; set; } = string.Empty;

    public bool IsMain { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Profile? Profile { get; set; }
}