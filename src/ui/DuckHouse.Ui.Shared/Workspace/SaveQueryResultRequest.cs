using DuckHouse.Core.Kernels;

namespace DuckHouse.Ui.Shared.Workspace;

public record SaveQueryResultRequest(
    string Status,
    double DurationMs,
    DataFrameOutput? DataFrame,
    string? Text,
    ErrorInfo? Error);
