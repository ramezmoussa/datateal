namespace DuckHouse.Ui.Shared.Workspace;

public record UpdateNotebookRequest(string Title, string Content, Guid? FolderId = null);
