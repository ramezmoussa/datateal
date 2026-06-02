using Datateal.ControlPlane.Core.Nodes;
using Datateal.ControlPlane.Core.Repositories;
using Datateal.Core.Mediator;

namespace Datateal.ControlPlane.Application.Mediator.Commands;

public record UpdateNodeConfigRequest(
    string Name,
    TimeSpan? KernelIdleTimeout,
    TimeSpan? NodeIdleTimeout) : IRequest;

internal class UpdateNodeConfigHandler(
    INodeConfigRepository nodeConfigRepository)
    : IRequestHandler<UpdateNodeConfigRequest>
{
    public async Task Handle(UpdateNodeConfigRequest request, CancellationToken cancellationToken)
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
    }
}
