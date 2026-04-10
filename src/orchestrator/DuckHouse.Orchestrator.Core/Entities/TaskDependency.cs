using DuckHouse.Orchestrator.Core.Enums;

namespace DuckHouse.Orchestrator.Core.Entities;

public class TaskDependency
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public JobTask? Task { get; set; }

    public Guid DependsOnTaskId { get; set; }
    public JobTask? DependsOnTask { get; set; }

    public DependencyCondition Condition { get; set; }
}
