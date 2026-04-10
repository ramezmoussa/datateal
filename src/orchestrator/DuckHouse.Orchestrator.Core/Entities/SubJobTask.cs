namespace DuckHouse.Orchestrator.Core.Entities;

public class SubJobTask : JobTask
{
    public Guid SubJobId { get; set; }
    public Dictionary<string, string>? Parameters { get; set; }
}
