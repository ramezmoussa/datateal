namespace DuckHouse.Core.Kernels;

/// <param name="Row">1-based line number.</param>
/// <param name="Col">0-based column number.</param>
/// <param name="Severity">Severity string: "error" or "warning".</param>
public record Diagnostic(int Row, int Col, string Message, string Severity);
