using Datateal.Core.Orchestration;

namespace Datateal.Orchestrator.Core.Entities;

public class NotebookTask : JobTask
{
    public NotebookTask() { TaskType = TaskType.Notebook; }

    public Guid NotebookId { get; set; }
    public string? NodePoolRef { get; set; }
    public Dictionary<string, string>? Parameters { get; set; }
}
