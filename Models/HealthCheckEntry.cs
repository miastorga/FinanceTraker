namespace PersonalFinanceTrackerAPI.Models;

public class HealthCheckEntry
{
    public string Status { get; set; }
    public string Description { get; set; }
    public string Duration { get; set; }
    public Dictionary<string, object> Data { get; set; }
    public string Exception { get; set; }
}