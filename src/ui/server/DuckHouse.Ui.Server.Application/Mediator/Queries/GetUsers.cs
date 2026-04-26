using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;
using DuckHouse.Ui.Shared.Users;

namespace DuckHouse.Ui.Server.Application.Mediator.Queries;

public record GetUsersRequest : IRequest<IReadOnlyList<AppUserDto>>;

internal class GetUsersHandler(IUserRepository repository) : IRequestHandler<GetUsersRequest, IReadOnlyList<AppUserDto>>
{
    public async Task<IReadOnlyList<AppUserDto>> Handle(GetUsersRequest request, CancellationToken cancellationToken)
    {
        var users = await repository.GetAllAsync(cancellationToken);
        return users.Select(Commands.UserDtoMapper.ToDto).ToList();
    }
}
