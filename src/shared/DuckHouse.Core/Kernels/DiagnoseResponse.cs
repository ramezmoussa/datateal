namespace DuckHouse.Core.Kernels;

public record DiagnoseResponse(IReadOnlyList<Diagnostic> Diagnostics);
