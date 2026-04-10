using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;
using DuckHouse.Ui.Shared.Workspace;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record UpdateQueryRequest(Guid Id, string Title, string Content, Guid? FolderId) : IRequest<QuerySummary?>;

internal class UpdateQueryHandler(IWorkspaceRepository repository) : IRequestHandler<UpdateQueryRequest, QuerySummary?>
{
    public async Task<QuerySummary?> Handle(UpdateQueryRequest request, CancellationToken cancellationToken)
    {
        var item = await repository.UpdateItemAsync(request.Id, request.Title, request.Content, request.FolderId, cancellationToken);
        return item is null ? null : new QuerySummary(item.Id, item.Title, item.FolderId, item.CreatedAt, item.UpdatedAt);
    }
}
