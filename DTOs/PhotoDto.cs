namespace DatingAppAPI.DTOs;

public class PhotoDto
{
    public int Id { get; set; }

    public int ProfileId { get; set; }

    public string Url { get; set; } = string.Empty;

    public bool IsMain { get; set; }

    public DateTime CreatedAt { get; set; }
}