namespace DuckHouse.Orchestrator.Core.Entities;

public class SqlQueryTask : JobTask
{
    public Guid QueryId { get; set; }
    public string? NodePoolRef { get; set; }
    public Dictionary<string, string>? Parameters { get; set; }
}
