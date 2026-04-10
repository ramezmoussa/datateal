using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record DeleteQueryRequest(Guid Id) : IRequest<bool>;

internal class DeleteQueryHandler(IWorkspaceRepository repository) : IRequestHandler<DeleteQueryRequest, bool>
{
    public Task<bool> Handle(DeleteQueryRequest request, CancellationToken cancellationToken) =>
        repository.DeleteQueryAsync(request.Id, cancellationToken);
}
