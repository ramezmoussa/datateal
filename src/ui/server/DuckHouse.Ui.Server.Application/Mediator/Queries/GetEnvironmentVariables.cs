using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;
using DuckHouse.Ui.Shared.Environment;

namespace DuckHouse.Ui.Server.Application.Mediator.Queries;

public record GetEnvironmentVariablesRequest : IRequest<IReadOnlyList<EnvironmentVariableDto>>;

internal class GetEnvironmentVariablesHandler(IEnvironmentRepository repository)
    : IRequestHandler<GetEnvironmentVariablesRequest, IReadOnlyList<EnvironmentVariableDto>>
{
    public async Task<IReadOnlyList<EnvironmentVariableDto>> Handle(GetEnvironmentVariablesRequest request, CancellationToken cancellationToken)
    {
        var variables = await repository.GetVariablesAsync(cancellationToken);
        return variables.Select(v => new EnvironmentVariableDto(
            v.Id, v.Key, v.Value, v.Description, v.CreatedAt, v.UpdatedAt)).ToList();
    }
}
