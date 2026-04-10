namespace DuckHouse.Ui.Shared.Workspace;

public record UpdateQueryRequest(string Title, string Content, Guid? FolderId = null);
