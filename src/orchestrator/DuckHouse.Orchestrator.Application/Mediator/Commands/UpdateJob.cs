using DuckHouse.Core.Mediator;
using DuckHouse.Orchestrator.Application.Validation;
using DuckHouse.Orchestrator.Core.Entities;
using DuckHouse.Orchestrator.Core.Enums;
using DuckHouse.Orchestrator.Core.Repositories;

namespace DuckHouse.Orchestrator.Application.Mediator.Commands;

public record UpdateJobRequest(
    Guid Id,
    string Name,
    string? Description,
    Guid? FolderId,
    int MaxConcurrentRuns,
    bool IsEnabled,
    List<UpdateJobTaskRequest>? Tasks = null,
    List<UpdateJobParameterRequest>? Parameters = null) : IRequest<Job?>;

public record UpdateJobTaskRequest(
    string Name,
    string TaskType,
    int MaxRetries,
    TimeSpan RetryInterval,
    TimeSpan? Timeout,
    Guid? NotebookId,
    Guid? QueryId,
    Guid? SubJobId,
    string? NodePoolRef,
    Dictionary<string, string>? Parameters,
    List<UpdateJobDependencyRequest> Dependencies);

public record UpdateJobDependencyRequest(string DependsOnTaskName, DependencyCondition Condition);

public record UpdateJobParameterRequest(string Name, string? DefaultValue, bool IsRequired, string? Description);

internal class UpdateJobHandler(IJobRepository jobRepository) : IRequestHandler<UpdateJobRequest, Job?>
{
    public async Task<Job?> Handle(UpdateJobRequest request, CancellationToken cancellationToken)
    {
        var existing = await jobRepository.GetJobAsync(request.Id, cancellationToken);
        if (existing is null) return null;

        // Update top-level settings
        existing.Name = request.Name;
        existing.Description = request.Description;
        existing.FolderId = request.FolderId;
        existing.MaxConcurrentRuns = request.MaxConcurrentRuns;
        existing.IsEnabled = request.IsEnabled;

        // Replace parameters if provided
        if (request.Parameters is not null)
        {
            existing.Parameters.Clear();
            foreach (var p in request.Parameters)
            {
                existing.Parameters.Add(new JobParameter
                {
                    Id = Guid.NewGuid(),
                    JobId = existing.Id,
                    Name = p.Name,
                    DefaultValue = p.DefaultValue,
                    IsRequired = p.IsRequired,
                    Description = p.Description,
                });
            }
        }

        // Replace tasks if provided
        if (request.Tasks is not null)
        {
            existing.Tasks.Clear();
            var tasksByName = new Dictionary<string, JobTask>(StringComparer.OrdinalIgnoreCase);

            foreach (var t in request.Tasks)
            {
                JobTask task = t.TaskType.ToLowerInvariant() switch
                {
                    "notebook" => new NotebookTask
                    {
                        Id = Guid.NewGuid(),
                        JobId = existing.Id,
                        Name = t.Name,
                        MaxRetries = t.MaxRetries,
                        RetryInterval = t.RetryInterval,
                        Timeout = t.Timeout,
                        NotebookId = t.NotebookId ?? throw new InvalidOperationException("NotebookId is required for notebook tasks."),
                        NodePoolRef = t.NodePoolRef,
                        Parameters = t.Parameters,
                    },
                    "sqlquery" or "sql" => new SqlQueryTask
                    {
                        Id = Guid.NewGuid(),
                        JobId = existing.Id,
                        Name = t.Name,
                        MaxRetries = t.MaxRetries,
                        RetryInterval = t.RetryInterval,
                        Timeout = t.Timeout,
                        QueryId = t.QueryId ?? throw new InvalidOperationException("QueryId is required for SQL query tasks."),
                        NodePoolRef = t.NodePoolRef,
                        Parameters = t.Parameters,
                    },
                    "subjob" => new SubJobTask
                    {
                        Id = Guid.NewGuid(),
                        JobId = existing.Id,
                        Name = t.Name,
                        MaxRetries = t.MaxRetries,
                        RetryInterval = t.RetryInterval,
                        Timeout = t.Timeout,
                        SubJobId = t.SubJobId ?? throw new InvalidOperationException("SubJobId is required for sub-job tasks."),
                        Parameters = t.Parameters,
                    },
                    _ => throw new InvalidOperationException($"Unknown task type: {t.TaskType}")
                };

                tasksByName[t.Name] = task;
                existing.Tasks.Add(task);
            }

            // Resolve dependencies by task name
            for (var i = 0; i < request.Tasks.Count; i++)
            {
                var taskReq = request.Tasks[i];
                var task = existing.Tasks[i];

                foreach (var dep in taskReq.Dependencies)
                {
                    if (!tasksByName.TryGetValue(dep.DependsOnTaskName, out var dependsOnTask))
                        throw new InvalidOperationException($"Task '{task.Name}' depends on unknown task '{dep.DependsOnTaskName}'.");

                    task.Dependencies.Add(new TaskDependency
                    {
                        Id = Guid.NewGuid(),
                        TaskId = task.Id,
                        DependsOnTaskId = dependsOnTask.Id,
                        Condition = dep.Condition,
                    });
                }
            }

            DagValidator.Validate(existing.Tasks);
        }

        return await jobRepository.UpdateJobAsync(existing, cancellationToken);
    }
}
