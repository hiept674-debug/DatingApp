namespace DatingAppAPI.DTOs.Interests;

public class UpdateInterestDto : CreateInterestDto
{
    public bool IsActive { get; set; } = true;
}