using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;
using DuckHouse.Ui.Shared.Environment;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record UpdateEnvironmentVariableRequest(Guid Id, string Key, string Value, string? Description)
    : IRequest<EnvironmentVariableDto?>;

internal class UpdateEnvironmentVariableHandler(IEnvironmentRepository repository)
    : IRequestHandler<UpdateEnvironmentVariableRequest, EnvironmentVariableDto?>
{
    public async Task<EnvironmentVariableDto?> Handle(UpdateEnvironmentVariableRequest request, CancellationToken cancellationToken)
    {
        var variable = await repository.UpdateVariableAsync(request.Id, request.Key, request.Value, request.Description, cancellationToken);
        if (variable is null) return null;
        return new EnvironmentVariableDto(variable.Id, variable.Key, variable.Value, variable.Description, variable.CreatedAt, variable.UpdatedAt);
    }
}
