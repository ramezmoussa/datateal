namespace DuckHouse.Ui.Shared.Workspace;

public record FolderSummary(Guid Id, string Name, Guid? ParentId, DateTime CreatedAt);
