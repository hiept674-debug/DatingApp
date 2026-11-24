namespace DatingAppAPI.DTOs.Like;

public class CreateLikeDto
{
    public int ToUserId { get; set; }

    public bool IsLike { get; set; }
}