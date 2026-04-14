using DuckHouse.Ui.Shared.Environment;

namespace DuckHouse.Ui.Client.Services;

public interface IEnvironmentService
{
    // Variables
    Task<IReadOnlyList<EnvironmentVariableDto>> GetVariablesAsync(CancellationToken ct = default);
    Task<EnvironmentVariableDto> CreateVariableAsync(CreateEnvironmentVariableRequest request, CancellationToken ct = default);
    Task<EnvironmentVariableDto?> UpdateVariableAsync(Guid id, UpdateEnvironmentVariableRequest request, CancellationToken ct = default);
    Task DeleteVariableAsync(Guid id, CancellationToken ct = default);

    // Secrets
    Task<IReadOnlyList<SecretDto>> GetSecretsAsync(CancellationToken ct = default);
    Task<SecretDto> CreateSecretAsync(CreateSecretRequest request, CancellationToken ct = default);
    Task<SecretDto?> UpdateSecretAsync(Guid id, UpdateSecretRequest request, CancellationToken ct = default);
    Task DeleteSecretAsync(Guid id, CancellationToken ct = default);
}
