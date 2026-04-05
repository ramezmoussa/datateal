namespace DuckHouse.Core.Kernels;

public record CompleteResponse(IReadOnlyList<CompletionItem> Items);
