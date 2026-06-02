namespace Datateal.Orchestrator.Core.Enums;

public enum TaskRunStatus
{
    Pending,
    Waiting,
    Running,
    Succeeded,
    Failed,
    Skipped,
    Cancelled,
    Retrying
}
