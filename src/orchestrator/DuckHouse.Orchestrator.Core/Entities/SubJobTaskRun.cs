using DuckHouse.Core.Orchestration;

namespace DuckHouse.Orchestrator.Core.Entities;

public class SubJobTaskRun : TaskRun
{
    public SubJobTaskRun() { TaskType = TaskType.SubJob; }
}
