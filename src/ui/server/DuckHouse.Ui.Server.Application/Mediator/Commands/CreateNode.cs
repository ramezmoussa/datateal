using DuckHouse.Core.Mediator;
using DuckHouse.Core.Nodes;
using DuckHouse.Ui.Server.Core.Repositories;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record CreateNodeRequest(
    string Name,
    string VmSize,
    TimeSpan? KernelIdleTimeout,
    TimeSpan? NodeIdleTimeout,
    string? KernelRequirements,
    IReadOnlyList<WheelContent>? WheelContents,
    IReadOnlyDictionary<string, string>? EnvironmentVariables,
    IReadOnlyDictionary<string, string>? Secrets) : IRequest<NodeInfo>;

internal class CreateNodeHandler(INodeRepository nodeRepository) : IRequestHandler<CreateNodeRequest, NodeInfo>
{
    public Task<NodeInfo> Handle(CreateNodeRequest request, CancellationToken cancellationToken) =>
        nodeRepository.CreateNodeAsync(
            request.Name,
            request.VmSize,
            request.KernelIdleTimeout,
            request.NodeIdleTimeout,
            request.KernelRequirements,
            request.WheelContents,
            request.EnvironmentVariables,
            request.Secrets,
            cancellationToken);
}