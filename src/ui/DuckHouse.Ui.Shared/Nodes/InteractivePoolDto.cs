using DuckHouse.Core.Nodes;

namespace DuckHouse.Ui.Shared.Nodes;

public record InteractivePoolDto(
    Guid Id,
    string Name,
    string NodeName,
    string VmSize,
    TimeSpan? KernelIdleTimeout,
    TimeSpan? NodeIdleTimeout,
    string? Description,
    NodeState? NodeState);
