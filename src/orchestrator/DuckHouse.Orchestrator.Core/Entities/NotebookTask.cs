namespace DuckHouse.Orchestrator.Core.Entities;

public class NotebookTask : JobTask
{
    public Guid NotebookId { get; set; }
    public string? NodePoolRef { get; set; }
    public Dictionary<string, string>? Parameters { get; set; }
}
