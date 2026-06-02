namespace Datateal.Ui.Shared.Workspace;

public record UpdateQueryRequest(string Title, string Content, Guid? FolderId, QueryLastResult? LastResult);
