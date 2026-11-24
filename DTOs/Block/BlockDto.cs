namespace DatingAppAPI.DTOs.Block;

public class BlockDto
{
    public int Id { get; set; }

    public int BlockerId { get; set; }

    public int BlockedUserId { get; set; }

    public DateTime CreatedAt { get; set; }
}