using Datateal.Core.Orchestration;

namespace Datateal.Orchestrator.Core.Entities;

public class SubJobTaskRun : TaskRun
{
    public SubJobTaskRun() { TaskType = TaskType.SubJob; }
}
