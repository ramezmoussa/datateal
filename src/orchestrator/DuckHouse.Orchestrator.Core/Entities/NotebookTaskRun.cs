using DuckHouse.Core.Orchestration;

namespace DuckHouse.Orchestrator.Core.Entities;

public class NotebookTaskRun : ComputeTaskRun
{
    public NotebookTaskRun() { TaskType = TaskType.Notebook; }
}
