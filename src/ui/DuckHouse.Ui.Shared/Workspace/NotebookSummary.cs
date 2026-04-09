namespace DuckHouse.Ui.Shared.Workspace;

public record NotebookSummary(Guid Id, string Title, Guid? FolderId, DateTime CreatedAt, DateTime UpdatedAt);
