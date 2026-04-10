namespace DuckHouse.Ui.Shared.Orchestration;

public record NodePoolConfigDto(
    Guid Id,
    string Name,
    string VmSize,
    TimeSpan? KernelIdleTimeout,
    TimeSpan? NodeIdleTimeout,
    string? KernelRequirements,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record CreateNodePoolRequest(
    string Name,
    string VmSize,
    TimeSpan? KernelIdleTimeout,
    TimeSpan? NodeIdleTimeout,
    string? KernelRequirements,
    string? Description);

public record UpdateNodePoolRequest(
    string Name,
    string VmSize,
    TimeSpan? KernelIdleTimeout,
    TimeSpan? NodeIdleTimeout,
    string? KernelRequirements,
    string? Description);
