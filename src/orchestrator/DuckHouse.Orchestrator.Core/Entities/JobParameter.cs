namespace DuckHouse.Orchestrator.Core.Entities;

public class JobParameter
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    public required string Name { get; set; }
    public string? DefaultValue { get; set; }
    public bool IsRequired { get; set; }
    public string? Description { get; set; }
}
