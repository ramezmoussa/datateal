using Datateal.Core.Mediator;
using Datateal.Ui.Server.Core.Repositories;

namespace Datateal.Ui.Server.Application.Mediator.Commands;

public record DeleteUserCommand(Guid Id) : IRequest<bool>;

internal class DeleteUserHandler(IUserRepository repository) : IRequestHandler<DeleteUserCommand, bool>
{
    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken) =>
        await repository.DeleteAsync(request.Id, cancellationToken);
}
