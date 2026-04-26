using DuckHouse.Ui.Shared.Users;

namespace DuckHouse.Ui.Client.Services;

public interface IUserService
{
    Task<IReadOnlyList<AppUserDto>> GetUsersAsync(CancellationToken ct = default);
    Task<AppUserDto?> GetUserAsync(Guid id, CancellationToken ct = default);
    Task<AppUserDto> CreateUserAsync(CreateUserRequest request, CancellationToken ct = default);
    Task<AppUserDto?> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default);
    Task DeleteUserAsync(Guid id, CancellationToken ct = default);
}
