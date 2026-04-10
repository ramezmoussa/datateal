using DuckHouse.Orchestrator.Core.Entities;
using DuckHouse.Orchestrator.Core.Enums;
using DuckHouse.Orchestrator.Core.Interfaces;
using DuckHouse.Orchestrator.Core.Repositories;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DuckHouse.Orchestrator.Application.Yaml;

/// <summary>
/// Converts a <see cref="Job"/> entity into YAML text.
/// </summary>
public class YamlJobSerializer(IWorkspaceReader workspaceReader, IJobRepository jobRepository)
{
    private static readonly ISerializer Serializer = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
        .Build();

    public async Task<string> SerializeAsync(Job job, CancellationToken ct = default)
    {
        var model = new YamlJobModel
        {
            Name = job.Name,
            Description = job.Description,
            MaxConcurrentRuns = job.MaxConcurrentRuns,
        };

        // Parameters
        foreach (var p in job.Parameters)
        {
            model.Parameters.Add(new YamlParameterModel
            {
                Name = p.Name,
                DefaultValue = p.DefaultValue,
                Required = p.IsRequired,
                Description = p.Description,
            });
        }

        // Collect distinct node pool refs used by tasks
        var nodePoolRefs = job.Tasks
            .Select(t => t switch
            {
                NotebookTask nb => nb.NodePoolRef,
                SqlQueryTask sq => sq.NodePoolRef,
                _ => null,
            })
            .Where(r => r is not null)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var refName in nodePoolRefs)
        {
            model.NodePools.Add(new YamlNodePoolModel { Name = refName! });
        }

        // Build task-id-to-name lookup for dependency resolution
        var taskNameById = job.Tasks.ToDictionary(t => t.Id, t => t.Name);

        // Tasks
        foreach (var task in job.Tasks)
        {
            var yamlTask = new YamlTaskModel
            {
                Name = task.Name,
                MaxRetries = task.MaxRetries,
                RetryInterval = task.RetryInterval != TimeSpan.Zero
                    ? task.RetryInterval.ToString()
                    : null,
                Timeout = task.Timeout?.ToString(),
            };

            switch (task)
            {
                case NotebookTask nb:
                    yamlTask.Type = "notebook";
                    yamlTask.NotebookPath = await workspaceReader.ResolveNotebookPathByIdAsync(nb.NotebookId, ct);
                    yamlTask.NodePoolRef = nb.NodePoolRef;
                    yamlTask.Parameters = nb.Parameters;
                    break;
                case SqlQueryTask sq:
                    yamlTask.Type = "sqlQuery";
                    yamlTask.QueryPath = await workspaceReader.ResolveQueryPathByIdAsync(sq.QueryId, ct);
                    yamlTask.NodePoolRef = sq.NodePoolRef;
                    yamlTask.Parameters = sq.Parameters;
                    break;
                case SubJobTask sj:
                    yamlTask.Type = "subJob";
                    var subJob = await jobRepository.GetJobAsync(sj.SubJobId, ct);
                    yamlTask.JobName = subJob?.Name;
                    yamlTask.Parameters = sj.Parameters;
                    break;
            }

            foreach (var dep in task.Dependencies)
            {
                yamlTask.Dependencies.Add(new YamlDependencyModel
                {
                    Task = taskNameById.TryGetValue(dep.DependsOnTaskId, out var name) ? name : dep.DependsOnTaskId.ToString(),
                    Condition = dep.Condition switch
                    {
                        DependencyCondition.OnSuccess => "onSuccess",
                        DependencyCondition.OnFailure => "onFailure",
                        DependencyCondition.OnCompletion => "onCompletion",
                        DependencyCondition.OnSkip => "onSkip",
                        _ => "onSuccess",
                    },
                });
            }

            model.Tasks.Add(yamlTask);
        }

        // Schedules
        foreach (var s in job.Schedules)
        {
            model.Schedules.Add(new YamlScheduleModel
            {
                Cron = s.CronExpression,
                TimeZone = s.TimeZone,
                Parameters = s.Parameters,
            });
        }

        return Serializer.Serialize(model);
    }
}
