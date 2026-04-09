namespace DuckHouse.Ui.Shared.Workspace;

public record CreateNotebookRequest(string Title, string Content, Guid? FolderId = null);
