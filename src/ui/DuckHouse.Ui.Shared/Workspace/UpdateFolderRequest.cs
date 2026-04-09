namespace DuckHouse.Ui.Shared.Workspace;

public record UpdateFolderRequest(string Name, Guid? ParentId = null);
