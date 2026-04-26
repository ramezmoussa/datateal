using System.Net.Http.Json;
using DuckHouse.Ui.Shared.Users;

namespace DuckHouse.Ui.Client.Services;

internal class UserService(HttpClient httpClient) : IUserService
{
    public async Task<IReadOnlyList<AppUserDto>> GetUsersAsync(CancellationToken ct = default) =>
        await httpClient.GetFromJsonAsync<IReadOnlyList<AppUserDto>>("api/users", ct) ?? [];

    public async Task<AppUserDto?> GetUserAsync(Guid id, CancellationToken ct = default) =>
        await httpClient.GetFromJsonAsync<AppUserDto>($"api/users/{id}", ct);

    public async Task<AppUserDto> CreateUserAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/users", request, ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AppUserDto>(ct))!;
    }

    public async Task<AppUserDto?> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default)
    {
        var response = await httpClient.PutAsJsonAsync($"api/users/{id}", request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AppUserDto>(ct);
    }

    public async Task DeleteUserAsync(Guid id, CancellationToken ct = default)
    {
        var response = await httpClient.DeleteAsync($"api/users/{id}", ct);
        response.EnsureSuccessStatusCode();
    }
}
