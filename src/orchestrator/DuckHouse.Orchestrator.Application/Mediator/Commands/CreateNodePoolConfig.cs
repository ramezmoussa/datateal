using DuckHouse.Core.Mediator;
using DuckHouse.Orchestrator.Application.Validation;
using DuckHouse.Orchestrator.Core.Entities;
using DuckHouse.Orchestrator.Core.Repositories;

namespace DuckHouse.Orchestrator.Application.Mediator.Commands;

public record CreateNodePoolConfigRequest(
    string Name,
    string PoolType,
    string VmSize,
    TimeSpan? KernelIdleTimeout,
    TimeSpan? NodeIdleTimeout,
    string? KernelRequirements,
    string? Description,
    List<Guid>? WheelPackageIds,
    List<Guid>? EnvironmentVariableIds,
    List<Guid>? SecretIds) : IRequest<NodePoolConfig>;

internal class CreateNodePoolConfigHandler(INodePoolConfigRepository repository)
    : IRequestHandler<CreateNodePoolConfigRequest, NodePoolConfig>
{
    public async Task<NodePoolConfig> Handle(CreateNodePoolConfigRequest request, CancellationToken cancellationToken)
    {
        var nameError = NodeNameValidator.ValidateNodePoolName(request.Name);
        if (nameError is not null)
            throw new ArgumentException(nameError, nameof(request.Name));

        NodePoolConfig config = request.PoolType == "Interactive"
            ? new InteractiveNodePoolConfig
            {
                Name = request.Name,
                VmSize = request.VmSize,
                KernelIdleTimeout = request.KernelIdleTimeout,
                NodeIdleTimeout = request.NodeIdleTimeout,
                KernelRequirements = request.KernelRequirements,
                Description = request.Description,
                WheelPackageIds = request.WheelPackageIds,
                EnvironmentVariableIds = request.EnvironmentVariableIds,
                SecretIds = request.SecretIds,
            }
            : new JobNodePoolConfig
            {
                Name = request.Name,
                VmSize = request.VmSize,
                KernelIdleTimeout = request.KernelIdleTimeout,
                NodeIdleTimeout = request.NodeIdleTimeout,
                KernelRequirements = request.KernelRequirements,
                Description = request.Description,
                WheelPackageIds = request.WheelPackageIds,
                EnvironmentVariableIds = request.EnvironmentVariableIds,
                SecretIds = request.SecretIds,
            };

        return await repository.CreateAsync(config, cancellationToken);
    }
}
