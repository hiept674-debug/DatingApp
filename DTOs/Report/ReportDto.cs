namespace DatingAppAPI.DTOs.Report;

public class ReportDto
{
    public int Id { get; set; }

    public int ReporterId { get; set; }

    public int ReportedUserId { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Status { get; set; } = string.Empty;

    public int? ReviewedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ReviewedAt { get; set; }
}