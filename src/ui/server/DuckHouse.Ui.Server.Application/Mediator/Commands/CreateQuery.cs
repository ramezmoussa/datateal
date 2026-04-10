using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;
using DuckHouse.Ui.Server.Core.Workspace;
using DuckHouse.Ui.Shared.Workspace;
using CoreItemType = DuckHouse.Ui.Server.Core.Workspace.WorkspaceItemType;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record CreateQueryRequest(string Title, string Content, Guid? FolderId) : IRequest<QuerySummary>;

internal class CreateQueryHandler(IWorkspaceRepository repository) : IRequestHandler<CreateQueryRequest, QuerySummary>
{
    public async Task<QuerySummary> Handle(CreateQueryRequest request, CancellationToken cancellationToken)
    {
        var item = await repository.CreateItemAsync(request.Title, request.Content, CoreItemType.Query, request.FolderId, cancellationToken);
        return new QuerySummary(item.Id, item.Title, item.FolderId, item.CreatedAt, item.UpdatedAt);
    }
}
