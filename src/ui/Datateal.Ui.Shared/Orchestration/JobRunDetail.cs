using Datateal.Core.Orchestration;

namespace Datateal.Ui.Shared.Orchestration;

public record JobRunDetail(
    Guid Id,
    Guid? JobId,
    string JobName,
    JobRunStatus Status,
    JobRunTrigger Trigger,
    DateTime CreatedAt,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    Dictionary<string, string>? Parameters,
    List<TaskRunDto> TaskRuns);
