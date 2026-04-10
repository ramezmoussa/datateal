namespace DuckHouse.Orchestrator.Core.Interfaces;

public record WorkspaceItemContent(Guid Id, string Title, string Content);

public interface IWorkspaceReader
{
    Task<WorkspaceItemContent?> GetNotebookContentAsync(Guid notebookId, CancellationToken ct = default);
    Task<WorkspaceItemContent?> GetQueryContentAsync(Guid queryId, CancellationToken ct = default);
}
