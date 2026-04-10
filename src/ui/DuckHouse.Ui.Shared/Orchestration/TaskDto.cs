namespace DuckHouse.Ui.Shared.Orchestration;

public record TaskDto(
    Guid Id,
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
    List<TaskDependencyDto> Dependencies);

public record TaskDependencyDto(Guid DependsOnTaskId, string DependsOnTaskName, string Condition);
