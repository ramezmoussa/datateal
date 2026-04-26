using DuckHouse.Core.Mediator;
using DuckHouse.Core.Users;
using DuckHouse.Ui.Server.Core.Repositories;
using DuckHouse.Ui.Shared.Users;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record CreateUserCommand(string Email, string DisplayName, List<string> Roles, bool HasAllCatalogAccess)
    : IRequest<AppUserDto>;

internal class CreateUserHandler(IUserRepository repository) : IRequestHandler<CreateUserCommand, AppUserDto>
{
    public async Task<AppUserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await repository.EmailExistsAsync(request.Email, ct: cancellationToken))
            throw new InvalidOperationException($"A user with email '{request.Email}' already exists.");

        var now = DateTime.UtcNow;
        var user = new AppUser
        {
            Id = Guid.CreateVersion7(),
            Email = request.Email,
            DisplayName = request.DisplayName,
            Roles = request.Roles,
            HasAllCatalogAccess = request.HasAllCatalogAccess,
            IsEnabled = true,
            CreatedAt = now,
            UpdatedAt = now,
        };

        await repository.CreateAsync(user, cancellationToken);
        return UserDtoMapper.ToDto(user);
    }
}

internal static class UserDtoMapper
{
    internal static AppUserDto ToDto(AppUser user) =>
        new(user.Id, user.Email, user.ExternalId, user.DisplayName,
            user.IsEnabled, user.HasAllCatalogAccess, user.Roles,
            user.CatalogAccessList.Select(a => new UserCatalogAccessDto(a.Id, a.CatalogId, a.Catalog?.Name ?? "")).ToList(),
            user.CreatedAt, user.UpdatedAt);
}
