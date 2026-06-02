using Datateal.Core.Workspace;

namespace Datateal.Ui.Shared.Workspace;

public record WorkspaceItemSummary(
    Guid Id,
    string Title,
    Guid? FolderId,
    WorkspaceItemType ItemType,
    DateTime CreatedAt,
    DateTime UpdatedAt);
