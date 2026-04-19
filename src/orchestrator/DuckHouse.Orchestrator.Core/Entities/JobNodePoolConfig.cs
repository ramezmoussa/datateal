namespace DuckHouse.Orchestrator.Core.Entities;

public class JobNodePoolConfig : NodePoolConfig
{
    public JobNodePoolConfig() => PoolType = "Job";
}
