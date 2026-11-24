
namespace DatingAppAPI.Models;

public class StoredFile
{
    public int Id { get; set; }

    public int? OwnerId { get; set; }

    public User? Owner { get; set; }

    public required string FileName { get; set; }

    public required string ContentType { get; set; }

    public required byte[] Content { get; set; }

    public long Length { get; set; }

    public string Purpose { get; set; } = "General";

    public DateTime UploadedAt { get; set; }
        = DateTime.UtcNow;
}
