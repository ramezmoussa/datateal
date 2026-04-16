namespace DuckHouse.Orchestrator.Core.Entities;

/// <summary>
/// Thrown when a job name is not unique across all jobs.
/// </summary>
public sealed class JobNameConflictException(string name)
    : InvalidOperationException($"A job named \"{name}\" already exists.")
{
    public string Name { get; } = name;
}
