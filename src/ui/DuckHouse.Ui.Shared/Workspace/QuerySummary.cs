namespace DuckHouse.Ui.Shared.Workspace;

public record QuerySummary(Guid Id, string Title, Guid? FolderId, DateTime CreatedAt, DateTime UpdatedAt);
