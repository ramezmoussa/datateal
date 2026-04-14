using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record DeleteSecretRequest(Guid Id) : IRequest<bool>;

internal class DeleteSecretHandler(IEnvironmentRepository repository)
    : IRequestHandler<DeleteSecretRequest, bool>
{
    public Task<bool> Handle(DeleteSecretRequest request, CancellationToken cancellationToken) =>
        repository.DeleteSecretAsync(request.Id, cancellationToken);
}
