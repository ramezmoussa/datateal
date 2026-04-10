namespace DuckHouse.Orchestrator.Core.Entities;

public class JobSchedule
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    public required string CronExpression { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string? TimeZone { get; set; }
    public Dictionary<string, string>? Parameters { get; set; }
    public DateTime? NextFireTime { get; set; }
}
