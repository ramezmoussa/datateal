using Datateal.Core.Kernels;

namespace Datateal.Ui.Shared.Workspace;

public record QueryLastResult(
    string Status,
    DateTime ExecutedAt,
    double DurationMs,
    DataFrameOutput? DataFrame,
    string? Text,
    ErrorInfo? Error);
