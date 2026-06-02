namespace Datateal.Ui.Shared.Orchestration;

public record JobDetail(
    Guid Id,
    string Name,
    string? Description,
    Guid? FolderId,
    bool IsEnabled,
    int MaxConcurrentRuns,
    List<TaskDto> Tasks,
    List<JobParameterDto> Parameters,
    List<ScheduleDto> Schedules,
    DateTime CreatedAt,
    DateTime UpdatedAt);
