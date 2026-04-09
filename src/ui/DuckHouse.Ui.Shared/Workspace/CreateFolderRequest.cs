namespace DuckHouse.Ui.Shared.Workspace;

public record CreateFolderRequest(string Name, Guid? ParentId = null);
