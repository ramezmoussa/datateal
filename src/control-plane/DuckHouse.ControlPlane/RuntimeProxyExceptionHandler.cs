using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace DuckHouse.ControlPlane;

// Propagates HTTP status codes from the duckhouse-runtime FastAPI (via the Kubernetes
// API server proxy) back to the caller as proper HTTP responses instead of 500s.
// For example, when the runtime returns 404 (kernel evicted), the control plane
// returns 404 rather than letting the unhandled HttpRequestException become a 500.
internal sealed class RuntimeProxyExceptionHandler(
    ILogger<RuntimeProxyExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not HttpRequestException { StatusCode: { } statusCode })
            return false;

        logger.LogWarning(
            "Runtime proxy returned {StatusCode}: {Message}",
            (int)statusCode, exception.Message);

        var title = environment.IsDevelopment()
            ? exception.Message
            : "An upstream service error occurred.";

        httpContext.Response.StatusCode = (int)statusCode;
        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails { Status = (int)statusCode, Title = title },
            cancellationToken: cancellationToken);
        return true;
    }
}
