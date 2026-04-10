namespace DuckHouse.Ui.Shared.Orchestration;

public record CreateJobRequest(
    string Name,
    string? Description,
    Guid? FolderId,
    int MaxConcurrentRuns);
