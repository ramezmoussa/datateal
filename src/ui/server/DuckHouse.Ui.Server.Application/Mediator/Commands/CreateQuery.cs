using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;
using DuckHouse.Ui.Server.Core.Workspace;
using DuckHouse.Ui.Shared.Workspace;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record CreateQueryRequest(string Title, string Content, Guid? FolderId) : IRequest<QuerySummary>;

internal class CreateQueryHandler(IWorkspaceRepository repository) : IRequestHandler<CreateQueryRequest, QuerySummary>
{
    public async Task<QuerySummary> Handle(CreateQueryRequest request, CancellationToken cancellationToken)
    {
        var query = await repository.CreateQueryAsync(request.Title, request.Content, request.FolderId, cancellationToken);
        return new QuerySummary(query.Id, query.Title, query.FolderId, query.CreatedAt, query.UpdatedAt);
    }
}
