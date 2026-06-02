namespace Datateal.Core.Kernels;

public record Output(string Type, string? Name, string? Text, Dictionary<string, object>? Data, int? ExecutionCount);
