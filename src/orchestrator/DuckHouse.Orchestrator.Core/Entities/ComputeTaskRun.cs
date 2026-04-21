using DuckHouse.Core.Orchestration;

namespace DuckHouse.Orchestrator.Core.Entities;

public abstract class ComputeTaskRun : TaskRun
{
    public string? NodeName { get; set; }
    public string? KernelId { get; set; }
    public string? OutputJson { get; set; }
}
