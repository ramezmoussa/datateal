using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;
using DuckHouse.Ui.Shared.Workspace;

namespace DuckHouse.Ui.Server.Application.Mediator.Queries;

public record GetFolderAncestorsRequest(Guid FolderId) : IRequest<IReadOnlyList<FolderSummary>>;

internal class GetFolderAncestorsHandler(IWorkspaceRepository repository)
    : IRequestHandler<GetFolderAncestorsRequest, IReadOnlyList<FolderSummary>>
{
    public async Task<IReadOnlyList<FolderSummary>> Handle(GetFolderAncestorsRequest request, CancellationToken cancellationToken)
    {
        var ancestors = await repository.GetFolderAncestorsAsync(request.FolderId, cancellationToken);
        return ancestors.Select(f => new FolderSummary(f.Id, f.Name, f.ParentId, f.CreatedAt)).ToList();
    }
}
