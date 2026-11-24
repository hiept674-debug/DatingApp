using System.ComponentModel.DataAnnotations;

namespace DatingAppAPI.DTOs.Users;

public class UpdateProfileDto
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [MaxLength(20)]
    public string Gender { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Bio { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;
}