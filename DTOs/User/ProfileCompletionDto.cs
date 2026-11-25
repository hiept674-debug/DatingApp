namespace DatingAppAPI.DTOs.Users;

public class ProfileCompletionDto
{
    public int Percentage { get; set; }

    public bool HasFullName { get; set; }

    public bool HasEmail { get; set; }

    public bool HasPhone { get; set; }

    public bool HasDateOfBirth { get; set; }

    public bool HasGender { get; set; }

    public bool HasCity { get; set; }

    public bool HasAvatar { get; set; }

    public bool HasBio { get; set; }

    public bool HasInterests { get; set; }

    public int InterestCount { get; set; }

    public bool HasBasicInformation =>
        HasFullName &&
        HasEmail &&
        HasPhone &&
        HasDateOfBirth &&
        HasGender &&
        HasCity;
}