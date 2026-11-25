using System.ComponentModel.DataAnnotations;

namespace DatingAppAPI.DTOs.Interests;

public class UpdateUserInterestsDto
{
    [Required]
    [MaxLength(10)]
    public List<int> InterestIds { get; set; } = new();
}