namespace DuckHouse.Ui.Shared.Workspace;

public record CreateQueryRequest(string Title, string Content, Guid? FolderId, QueryLastResult? LastResult);
