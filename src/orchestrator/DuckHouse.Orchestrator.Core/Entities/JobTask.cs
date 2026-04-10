namespace DuckHouse.Orchestrator.Core.Entities;

/// <summary>
/// Abstract base class for tasks in a job DAG. Uses TPH inheritance with discriminator "TaskType".
/// </summary>
public abstract class JobTask
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    public required string Name { get; set; }
    public int MaxRetries { get; set; }
    public TimeSpan RetryInterval { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan? Timeout { get; set; }

    public List<TaskDependency> Dependencies { get; set; } = [];
}
