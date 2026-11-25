namespace DatingAppAPI.Models;

public class DemoProfileInterest
{
    public int DemoProfileId { get; set; }

    public int InterestId { get; set; }

    public DemoProfile DemoProfile { get; set; } = null!;

    public Interest Interest { get; set; } = null!;
}