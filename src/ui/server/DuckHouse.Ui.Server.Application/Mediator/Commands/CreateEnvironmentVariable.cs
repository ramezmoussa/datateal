using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;
using DuckHouse.Ui.Shared.Environment;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record CreateEnvironmentVariableRequest(string Key, string Value, string? Description)
    : IRequest<EnvironmentVariableDto>;

internal class CreateEnvironmentVariableHandler(IEnvironmentRepository repository)
    : IRequestHandler<CreateEnvironmentVariableRequest, EnvironmentVariableDto>
{
    public async Task<EnvironmentVariableDto> Handle(CreateEnvironmentVariableRequest request, CancellationToken cancellationToken)
    {
        var variable = await repository.CreateVariableAsync(request.Key, request.Value, request.Description, cancellationToken);
        return new EnvironmentVariableDto(variable.Id, variable.Key, variable.Value, variable.Description, variable.CreatedAt, variable.UpdatedAt);
    }
}
