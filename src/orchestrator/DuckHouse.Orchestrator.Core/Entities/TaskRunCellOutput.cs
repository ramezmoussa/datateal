using System.Text.Json.Serialization;
using DuckHouse.Orchestrator.Core.Enums;

namespace DuckHouse.Orchestrator.Core.Entities;

public class TaskRunCellOutput
{
    public Guid Id { get; set; }
    public Guid TaskRunId { get; set; }
    [JsonIgnore]
    public TaskRun? TaskRun { get; set; }

    public int CellIndex { get; set; }
    public required string CellSource { get; set; }
    public required string CellType { get; set; }
    public string? Language { get; set; }
    public CellExecutionStatus Status { get; set; }
    public string? OutputsJson { get; set; }
    public string? ErrorJson { get; set; }
    public int? ExecutionCount { get; set; }
    public double? DurationMs { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
