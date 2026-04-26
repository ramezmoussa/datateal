using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;
using DuckHouse.Ui.Shared.Users;

namespace DuckHouse.Ui.Server.Application.Mediator.Queries;

public record GetUserRequest(Guid Id) : IRequest<AppUserDto?>;

internal class GetUserHandler(IUserRepository repository) : IRequestHandler<GetUserRequest, AppUserDto?>
{
    public async Task<AppUserDto?> Handle(GetUserRequest request, CancellationToken cancellationToken)
    {
        var user = await repository.GetByIdAsync(request.Id, cancellationToken);
        return user is not null ? Commands.UserDtoMapper.ToDto(user) : null;
    }
}
