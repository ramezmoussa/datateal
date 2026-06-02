using System.Text.Json.Serialization;
using Datateal.Orchestrator.Core.Enums;

namespace Datateal.Orchestrator.Core.Entities;

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
