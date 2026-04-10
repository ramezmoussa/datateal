using DuckHouse.Orchestrator.Core.Enums;

namespace DuckHouse.Orchestrator.Core.Entities;

public class JobRun
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    public JobRunStatus Status { get; set; }
    public JobRunTrigger Trigger { get; set; }
    public Guid? ScheduleId { get; set; }
    public Guid? ParentRunId { get; set; }
    public JobRun? ParentRun { get; set; }
    public Guid? ParentTaskRunId { get; set; }
    public string? ParametersJson { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public List<TaskRun> TaskRuns { get; set; } = [];
}
