namespace DuckHouse.Ui.Shared.Orchestration;

public record JobRunSummary(
    Guid Id,
    Guid JobId,
    string Status,
    string Trigger,
    DateTime CreatedAt,
    DateTime? StartedAt,
    DateTime? CompletedAt);
