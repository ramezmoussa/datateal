namespace Datateal.Core.Kernels;

public record PollExecutionResponse(bool IsComplete, ExecutionResult? Result = null);
