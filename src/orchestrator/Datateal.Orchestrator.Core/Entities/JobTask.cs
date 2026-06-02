using System.Text.Json.Serialization;
using Datateal.Core.Orchestration;

namespace Datateal.Orchestrator.Core.Entities;

/// <summary>
/// Abstract base class for tasks in a job DAG. Uses TPH inheritance with discriminator "TaskType".
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "taskType")]
[JsonDerivedType(typeof(NotebookTask), "Notebook")]
[JsonDerivedType(typeof(SqlQueryTask), "SqlQuery")]
[JsonDerivedType(typeof(SubJobTask), "SubJob")]
public abstract class JobTask
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    [JsonIgnore]
    public Job? Job { get; set; }

    [JsonIgnore]
    public TaskType TaskType { get; protected set; }

    public required string Name { get; set; }
    public int MaxRetries { get; set; }
    public TimeSpan RetryInterval { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan? Timeout { get; set; }

    public List<TaskDependency> Dependencies { get; set; } = [];
}
