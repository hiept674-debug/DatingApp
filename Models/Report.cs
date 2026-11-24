namespace DatingAppAPI.Models;

using System.ComponentModel.DataAnnotations;

public class Report
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ReporterId { get; set; }

    [Required]
    public int ReportedUserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Reason { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    public int? ReviewedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ReviewedAt { get; set; }

    public User Reporter { get; set; } = null!;

    public User ReportedUser { get; set; } = null!;

    public User? Reviewer { get; set; }
}