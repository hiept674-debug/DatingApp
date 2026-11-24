namespace DatingAppAPI.DTOs.Like;

public class LikeDto
{
    public int Id { get; set; }

    public int FromUserId { get; set; }

    public int ToUserId { get; set; }

    public bool IsLike { get; set; }

    public DateTime CreatedAt { get; set; }
}