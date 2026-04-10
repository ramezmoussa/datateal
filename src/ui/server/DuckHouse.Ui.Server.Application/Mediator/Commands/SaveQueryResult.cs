using System.Text.Json;
using DuckHouse.Core.Kernels;
using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record SaveQueryResultRequest(
    Guid Id,
    string Status,
    double DurationMs,
    DataFrameOutput? DataFrame,
    string? Text,
    ErrorInfo? Error) : IRequest<bool>;

internal class SaveQueryResultHandler(IWorkspaceRepository repository) : IRequestHandler<SaveQueryResultRequest, bool>
{
    public async Task<bool> Handle(SaveQueryResultRequest request, CancellationToken cancellationToken)
    {
        string? resultJson = null;
        if (request.DataFrame is not null || request.Text is not null || request.Error is not null)
        {
            resultJson = JsonSerializer.Serialize(new ResultPayload(request.DataFrame, request.Text, request.Error));
        }

        return await repository.SaveQueryResultAsync(
            request.Id,
            request.Status,
            request.DurationMs,
            DateTime.UtcNow,
            resultJson,
            cancellationToken);
    }

    private record ResultPayload(DataFrameOutput? DataFrame, string? Text, ErrorInfo? Error);
}
