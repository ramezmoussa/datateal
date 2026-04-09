using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;
using DuckHouse.Ui.Shared.Workspace;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record UpdateFolderRequest(Guid Id, string Name, Guid? ParentId) : IRequest<FolderSummary?>;

internal class UpdateFolderHandler(IWorkspaceRepository repository) : IRequestHandler<UpdateFolderRequest, FolderSummary?>
{
    public async Task<FolderSummary?> Handle(UpdateFolderRequest request, CancellationToken cancellationToken)
    {
        var folder = await repository.UpdateFolderAsync(request.Id, request.Name, request.ParentId, cancellationToken);
        return folder is null ? null : new FolderSummary(folder.Id, folder.Name, folder.ParentId, folder.CreatedAt);
    }
}
