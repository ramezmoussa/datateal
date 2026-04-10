using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;
using DuckHouse.Ui.Shared.Workspace;

namespace DuckHouse.Ui.Server.Application.Mediator.Queries;

public record GetQueryRequest(Guid Id) : IRequest<QueryDetail?>;

internal class GetQueryHandler(IWorkspaceRepository repository) : IRequestHandler<GetQueryRequest, QueryDetail?>
{
    public async Task<QueryDetail?> Handle(GetQueryRequest request, CancellationToken cancellationToken)
    {
        var item = await repository.GetItemAsync(request.Id, cancellationToken);
        return item is null
            ? null
            : new QueryDetail(item.Id, item.Title, item.FolderId, item.CreatedAt, item.UpdatedAt, item.Content);
    }
}
