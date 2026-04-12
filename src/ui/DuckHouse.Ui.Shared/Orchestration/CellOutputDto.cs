namespace DuckHouse.Ui.Shared.Orchestration;

public record CellOutputDto(
    Guid Id,
    int CellIndex,
    string CellSource,
    string CellType,
    string? Language,
    string Status,
    string? CellRole,
    string? OutputsJson,
    string? ErrorJson,
    int? ExecutionCount,
    double? DurationMs,
    DateTime? StartedAt,
    DateTime? CompletedAt);
