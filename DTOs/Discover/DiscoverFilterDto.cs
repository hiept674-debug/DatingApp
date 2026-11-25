using System.ComponentModel.DataAnnotations;

namespace DatingAppAPI.DTOs.Discover;

public class DiscoverFilterDto
{
    [MaxLength(100)]
    public string? Search { get; set; }

    [MaxLength(100)]
    public string? Category { get; set; }

    [MaxLength(10)]
    public List<int> InterestIds { get; set; } = new();

    public bool MatchAllInterests { get; set; } = false;

    public string SortBy { get; set; } = "newest";

    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;

    [Range(1, 50)]
    public int PageSize { get; set; } = 12;
}