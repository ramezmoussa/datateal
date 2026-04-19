using DuckHouse.ControlPlane.Application.InactivityEviction;
using DuckHouse.ControlPlane.Core.Nodes;
using DuckHouse.ControlPlane.Core.Repositories;
using DuckHouse.Core.Mediator;
using Microsoft.Extensions.Options;

namespace DuckHouse.ControlPlane.Application.Mediator.Commands;

public record UpdateNodeConfigRequest(
    string Name,
    TimeSpan? KernelIdleTimeout,
    TimeSpan? NodeIdleTimeout) : IRequest;

internal class UpdateNodeConfigHandler(
    INodeConfigRepository nodeConfigRepository,
    IOptions<InactivityEvictionOptions> evictionOptions)
    : IRequestHandler<UpdateNodeConfigRequest>
{
    public async Task Handle(UpdateNodeConfigRequest request, CancellationToken cancellationToken)
    {
        var opts = evictionOptions.Value;
        var config = new NodeConfig
        {
            NodeName = request.Name,
            KernelIdleTimeout = request.KernelIdleTimeout ?? opts.KernelIdleTimeout,
            NodeIdleTimeout = request.NodeIdleTimeout ?? opts.NodeIdleTimeout,
        };

        await nodeConfigRepository.UpsertAsync(config, cancellationToken);
    }
}
