namespace DuckHouse.Ui.Shared.Workspace;

public record WorkspaceListing(IReadOnlyList<FolderSummary> Folders, IReadOnlyList<NotebookSummary> Notebooks);
