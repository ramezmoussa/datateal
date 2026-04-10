namespace DuckHouse.Ui.Shared.Orchestration;

public record UpdateJobRequest(
    string Name,
    string? Description,
    Guid? FolderId,
    int MaxConcurrentRuns,
    bool IsEnabled);
