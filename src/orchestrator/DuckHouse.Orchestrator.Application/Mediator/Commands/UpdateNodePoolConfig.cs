using DuckHouse.Core.Mediator;
using DuckHouse.Orchestrator.Application.Validation;
using DuckHouse.Orchestrator.Core.Entities;
using DuckHouse.Orchestrator.Core.Interfaces;
using DuckHouse.Orchestrator.Core.Repositories;

namespace DuckHouse.Orchestrator.Application.Mediator.Commands;

public record UpdateNodePoolConfigRequest(
    Guid Id,
    string Name,
    string VmSize,
    TimeSpan? KernelIdleTimeout,
    TimeSpan? NodeIdleTimeout,
    string? KernelRequirements,
    string? Description,
    List<Guid>? WheelPackageIds = null,
    List<Guid>? EnvironmentVariableIds = null,
    List<Guid>? SecretIds = null) : IRequest<NodePoolConfig?>;

internal class UpdateNodePoolConfigHandler(
    INodePoolConfigRepository repository,
    IControlPlaneClient controlPlaneClient)
    : IRequestHandler<UpdateNodePoolConfigRequest, NodePoolConfig?>
{
    public async Task<NodePoolConfig?> Handle(UpdateNodePoolConfigRequest request, CancellationToken cancellationToken)
    {
        var nameError = NodeNameValidator.ValidateNodePoolName(request.Name);
        if (nameError is not null)
            throw new ArgumentException(nameError, nameof(request.Name));

        var existing = await repository.GetAsync(request.Id, cancellationToken);
        if (existing is null) return null;

        existing.Name = request.Name;
        existing.VmSize = request.VmSize;
        existing.KernelIdleTimeout = request.KernelIdleTimeout;
        existing.NodeIdleTimeout = request.NodeIdleTimeout;
        existing.KernelRequirements = request.KernelRequirements;
        existing.Description = request.Description;
        existing.WheelPackageIds = request.WheelPackageIds;
        existing.EnvironmentVariableIds = request.EnvironmentVariableIds;
        existing.SecretIds = request.SecretIds;

        var updated = await repository.UpdateAsync(existing, cancellationToken);

        if (existing is InteractiveNodePoolConfig interactive)
        {
            await controlPlaneClient.UpdateNodeEvictionConfigAsync(
                interactive.GetNodeName(),
                request.KernelIdleTimeout,
                request.NodeIdleTimeout,
                cancellationToken);
        }

        return updated;
    }
}
