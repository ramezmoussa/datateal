using Datateal.ControlPlane.Core.Nodes;
using Datateal.ControlPlane.Core.Repositories;
using Datateal.ControlPlane.Core.Services;
using Datateal.Core.Mediator;
using Datateal.Core.Nodes;

namespace Datateal.ControlPlane.Application.Mediator.Commands;

public record CreateNodeRequest(
    string Name,
    string? VmSize,
    TimeSpan? KernelIdleTimeout,
    TimeSpan? NodeIdleTimeout,
    string? KernelRequirements,
    IReadOnlyList<WheelContent>? WheelContents,
    IReadOnlyDictionary<string, string>? EnvironmentVariables,
    IReadOnlyDictionary<string, string>? Secrets) : IRequest<NodeInfo>;

internal class CreateNodeHandler(
    INodeService nodeService,
    INodeConfigRepository nodeConfigRepository) : IRequestHandler<CreateNodeRequest, NodeInfo>
{
    public async Task<NodeInfo> Handle(CreateNodeRequest request, CancellationToken cancellationToken)
    {
        // null = use the eviction service's global default at runtime.
        // TimeSpan.Zero = never evict. Positive = specific timeout.
        var config = new NodeConfig
        {
            NodeName = request.Name,
            KernelIdleTimeout = request.KernelIdleTimeout,
            NodeIdleTimeout = request.NodeIdleTimeout,
        };

        await nodeConfigRepository.UpsertAsync(config, cancellationToken);

        var node = await nodeService.CreateNodeAsync(
            new Datateal.Core.Nodes.CreateNodeRequest(request.Name, request.VmSize,
                KernelRequirements: request.KernelRequirements,
                WheelContents: request.WheelContents,
                EnvironmentVariables: request.EnvironmentVariables,
                Secrets: request.Secrets),
            cancellationToken);

        return node;
    }
}
