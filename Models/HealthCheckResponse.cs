namespace PersonalFinanceTrackerAPI.Models;

public class HealthCheckResponse
{
    public string Status { get; set; }
    public string TotalDuration { get; set; }
    public Dictionary<string, HealthCheckEntry> Entries { get; set; }
}