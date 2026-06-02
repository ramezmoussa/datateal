using Datateal.Core.Orchestration;

namespace Datateal.Orchestrator.Core.Entities;

public class NotebookTaskRun : ComputeTaskRun
{
    public NotebookTaskRun() { TaskType = TaskType.Notebook; }
}
