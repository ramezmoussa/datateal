using DuckHouse.Core.Kernels;

namespace DuckHouse.Ui.Shared.Workspace;

public record QueryLastResult(
    string Status,
    DateTime ExecutedAt,
    double DurationMs,
    DataFrameOutput? DataFrame,
    string? Text,
    ErrorInfo? Error);
