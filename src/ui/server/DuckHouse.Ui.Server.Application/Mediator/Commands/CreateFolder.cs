using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;
using DuckHouse.Ui.Shared.Workspace;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record CreateFolderRequest(string Name, Guid? ParentId) : IRequest<FolderSummary>;

internal class CreateFolderHandler(IWorkspaceRepository repository) : IRequestHandler<CreateFolderRequest, FolderSummary>
{
    public async Task<FolderSummary> Handle(CreateFolderRequest request, CancellationToken cancellationToken)
    {
        var folder = await repository.CreateFolderAsync(request.Name, request.ParentId, cancellationToken);
        return new FolderSummary(folder.Id, folder.Name, folder.ParentId, folder.CreatedAt);
    }
}
