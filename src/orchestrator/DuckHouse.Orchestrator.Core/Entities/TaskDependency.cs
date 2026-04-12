using System.Text.Json.Serialization;
using DuckHouse.Orchestrator.Core.Enums;

namespace DuckHouse.Orchestrator.Core.Entities;

public class TaskDependency
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    [JsonIgnore]
    public JobTask? Task { get; set; }

    public Guid DependsOnTaskId { get; set; }
    [JsonIgnore]
    public JobTask? DependsOnTask { get; set; }
    public string? DependsOnTaskName => DependsOnTask?.Name;

    public DependencyCondition Condition { get; set; }
}
