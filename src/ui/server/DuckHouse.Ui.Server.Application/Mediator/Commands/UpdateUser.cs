using DuckHouse.Core.Mediator;
using DuckHouse.Core.Users;
using DuckHouse.Ui.Server.Core.Repositories;
using DuckHouse.Ui.Shared.Users;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record UpdateUserCommand(
    Guid Id, string DisplayName, bool IsEnabled,
    List<string> Roles, bool HasAllCatalogAccess, List<Guid> CatalogIds)
    : IRequest<AppUserDto?>;

internal class UpdateUserHandler(IUserRepository repository) : IRequestHandler<UpdateUserCommand, AppUserDto?>
{
    public async Task<AppUserDto?> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing is null) return null;

        existing.DisplayName = request.DisplayName;
        existing.IsEnabled = request.IsEnabled;
        existing.Roles = request.Roles;
        existing.HasAllCatalogAccess = request.HasAllCatalogAccess;
        existing.UpdatedAt = DateTime.UtcNow;

        // Rebuild catalog access list
        existing.CatalogAccessList = request.CatalogIds
            .Select(catalogId => new UserCatalogAccess
            {
                Id = Guid.CreateVersion7(),
                UserId = existing.Id,
                CatalogId = catalogId,
            })
            .ToList();

        var updated = await repository.UpdateAsync(existing, cancellationToken);
        return updated is not null ? UserDtoMapper.ToDto(updated) : null;
    }
}
