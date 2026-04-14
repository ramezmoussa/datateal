using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;
using DuckHouse.Ui.Shared.Environment;

namespace DuckHouse.Ui.Server.Application.Mediator.Queries;

public record GetSecretsRequest : IRequest<IReadOnlyList<SecretDto>>;

internal class GetSecretsHandler(IEnvironmentRepository repository)
    : IRequestHandler<GetSecretsRequest, IReadOnlyList<SecretDto>>
{
    public async Task<IReadOnlyList<SecretDto>> Handle(GetSecretsRequest request, CancellationToken cancellationToken)
    {
        var secrets = await repository.GetSecretsAsync(cancellationToken);
        return secrets.Select(s => new SecretDto(
            s.Id, s.Key, s.Description, s.CreatedAt, s.UpdatedAt)).ToList();
    }
}
