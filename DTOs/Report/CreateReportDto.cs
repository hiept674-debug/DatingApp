namespace DatingAppAPI.DTOs.Report;

public class CreateReportDto
{
    public int ReportedUserId { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string? Description { get; set; }
}