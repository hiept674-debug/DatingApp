using System.ComponentModel.DataAnnotations;

namespace DatingAppAPI.DTOs.Discover;

public class DiscoverQueryParameters
{
    [MaxLength(100)]
    public string? Search { get; set; }

    public string SortBy { get; set; } = "newest";

    [Range(1, 100000)]
    public int PageNumber { get; set; } = 1;

    [Range(1, 24)]
    public int PageSize { get; set; } = 6;
}