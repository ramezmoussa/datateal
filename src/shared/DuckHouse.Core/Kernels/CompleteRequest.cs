namespace DuckHouse.Core.Kernels;

/// <param name="Line">1-based line number (Jedi convention).</param>
/// <param name="Column">0-based column number (Jedi convention).</param>
public record CompleteRequest(string Code, int Line, int Column);
