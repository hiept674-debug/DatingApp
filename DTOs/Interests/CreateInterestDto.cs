using System.ComponentModel.DataAnnotations;

namespace DatingAppAPI.DTOs.Interests;

public class CreateInterestDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Icon { get; set; }
}