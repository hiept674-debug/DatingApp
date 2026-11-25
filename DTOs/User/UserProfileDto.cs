namespace DatingAppAPI.DTOs.Users;

public class UserProfileDto
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public int Age { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string Bio { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Giữ tương thích với Frontend cũ
    public int ProfileCompletion { get; set; }

    // Chi tiết tỷ lệ hoàn thành
    public ProfileCompletionDto CompletionDetails
    { get; set; } = new();
}