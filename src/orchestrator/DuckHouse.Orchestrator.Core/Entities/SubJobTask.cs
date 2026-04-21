using DuckHouse.Core.Orchestration;

namespace DuckHouse.Orchestrator.Core.Entities;

public class SubJobTask : JobTask
{
    public SubJobTask() { TaskType = TaskType.SubJob; }

    public Guid SubJobId { get; set; }
    public Dictionary<string, string>? Parameters { get; set; }
}
