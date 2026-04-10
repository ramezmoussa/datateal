namespace DuckHouse.Ui.Client.Models;

public class EditableTask
{
    public string Name { get; set; } = "";
    public string TaskType { get; set; } = "Notebook";
    public int MaxRetries { get; set; }
    public string RetryInterval { get; set; } = "00:00:30";
    public string? Timeout { get; set; }

    public Guid? NotebookId { get; set; }
    public Guid? QueryId { get; set; }
    public Guid? SubJobId { get; set; }

    public string? NodePoolRef { get; set; }
    public List<EditableTaskParameter> Parameters { get; set; } = [];
    public List<EditableDependency> Dependencies { get; set; } = [];
}

public class EditableTaskParameter
{
    public string Key { get; set; } = "";
    public string Value { get; set; } = "";
}

public class EditableDependency
{
    public string TaskName { get; set; } = "";
    public string Condition { get; set; } = "OnSuccess";
}

public class EditableParameter
{
    public string Name { get; set; } = "";
    public string? DefaultValue { get; set; }
    public bool IsRequired { get; set; }
    public string? Description { get; set; }
}
