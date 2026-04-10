using DuckHouse.Orchestrator.Core.Entities;

namespace DuckHouse.Orchestrator.Application.Validation;

public static class DagValidator
{
    /// <summary>
    /// Validates that the task dependency graph is a valid DAG (no cycles) and that
    /// all dependency references point to tasks within the same job.
    /// Uses Kahn's algorithm for topological sort.
    /// Resolves dependencies via the <see cref="TaskDependency.DependsOnTask"/> navigation
    /// property first, falling back to <see cref="TaskDependency.DependsOnTaskId"/> FK lookup.
    /// </summary>
    public static void Validate(IReadOnlyList<JobTask> tasks)
    {
        // Build name-based graph (names are unique within a job)
        var inDegree = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var adjacency = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        // ID→Name lookup for FK-based resolution (when navigation is not set)
        var idToName = new Dictionary<Guid, string>();
        foreach (var task in tasks)
        {
            inDegree[task.Name] = 0;
            adjacency[task.Name] = [];
            if (task.Id != Guid.Empty)
                idToName[task.Id] = task.Name;
        }

        foreach (var task in tasks)
        {
            foreach (var dep in task.Dependencies)
            {
                // Prefer navigation property; fall back to FK lookup
                var depName = dep.DependsOnTask?.Name;
                if (depName is null && dep.DependsOnTaskId != Guid.Empty)
                    idToName.TryGetValue(dep.DependsOnTaskId, out depName);

                if (depName is null || !inDegree.ContainsKey(depName))
                    throw new InvalidOperationException(
                        $"Task '{task.Name}' has a dependency on a task that does not exist in the job.");

                adjacency[depName].Add(task.Name);
                inDegree[task.Name]++;
            }
        }

        // Kahn's algorithm — topological sort to detect cycles
        var queue = new Queue<string>();
        foreach (var (name, degree) in inDegree)
        {
            if (degree == 0)
                queue.Enqueue(name);
        }

        var sortedCount = 0;
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            sortedCount++;

            foreach (var neighbor in adjacency[current])
            {
                if (--inDegree[neighbor] == 0)
                    queue.Enqueue(neighbor);
            }
        }

        if (sortedCount != tasks.Count)
            throw new InvalidOperationException(
                "The task dependency graph contains a cycle. Please remove circular dependencies.");
    }
}
