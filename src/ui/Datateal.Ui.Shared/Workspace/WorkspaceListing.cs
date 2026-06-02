namespace Datateal.Ui.Shared.Workspace;

public record WorkspaceListing(
    IReadOnlyList<FolderSummary> Folders,
    IReadOnlyList<WorkspaceItemSummary> Items);
