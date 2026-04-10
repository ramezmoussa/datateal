using DuckHouse.Orchestrator.Core.Enums;

namespace DuckHouse.Orchestrator.Core.Entities;

public class TaskRun
{
    public Guid Id { get; set; }
    public Guid JobRunId { get; set; }
    public JobRun? JobRun { get; set; }

    public Guid TaskId { get; set; }
    public JobTask? Task { get; set; }

    public TaskRunStatus Status { get; set; }
    public int AttemptNumber { get; set; }
    public string? NodeName { get; set; }
    public string? KernelId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public double? DurationMs { get; set; }

    public string? NotebookOutputJson { get; set; }
    public string? QueryResultJson { get; set; }

    public List<TaskRunCellOutput> CellOutputs { get; set; } = [];
}
