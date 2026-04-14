using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record DeleteEnvironmentVariableRequest(Guid Id) : IRequest<bool>;

internal class DeleteEnvironmentVariableHandler(IEnvironmentRepository repository)
    : IRequestHandler<DeleteEnvironmentVariableRequest, bool>
{
    public Task<bool> Handle(DeleteEnvironmentVariableRequest request, CancellationToken cancellationToken) =>
        repository.DeleteVariableAsync(request.Id, cancellationToken);
}
