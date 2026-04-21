using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Quartz;

namespace DuckHouse.Orchestrator.Core.Entities;

public class JobSchedule
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    [JsonIgnore]
    public Job? Job { get; set; }

    public required string CronExpression { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string? TimeZone { get; set; }
    public Dictionary<string, string>? Parameters { get; set; }

    /// <summary>
    /// Computed from the cron expression and timezone. Not persisted to the database.
    /// Returns null if the schedule is disabled or the expression is invalid.
    /// </summary>
    [NotMapped]
    public DateTime? NextFireTime
    {
        get
        {
            if (!IsEnabled) return null;
            try
            {
                var cron = new CronExpression(CronExpression)
                {
                    TimeZone = !string.IsNullOrWhiteSpace(TimeZone)
                        ? TimeZoneInfo.FindSystemTimeZoneById(TimeZone)
                        : TimeZoneInfo.Local
                };
                return cron.GetNextValidTimeAfter(DateTimeOffset.UtcNow)?.UtcDateTime;
            }
            catch
            {
                return null;
            }
        }
    }
}
