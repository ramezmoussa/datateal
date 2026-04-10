namespace DuckHouse.Ui.Shared.Orchestration;

public record JobSummary(
    Guid Id,
    string Name,
    string? Description,
    bool IsEnabled,
    int MaxConcurrentRuns,
    int TaskCount,
    int ScheduleCount,
    DateTime CreatedAt,
    DateTime UpdatedAt);
