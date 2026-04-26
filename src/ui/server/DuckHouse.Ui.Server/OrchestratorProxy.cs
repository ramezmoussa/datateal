using DuckHouse.Auth.ApiKey;
using Microsoft.Extensions.Options;

namespace DuckHouse.Ui.Server;

public static class OrchestratorProxy
{
    public static IServiceCollection AddOrchestratorProxy(this IServiceCollection services, IConfiguration configuration)
    {
        var apiKey = configuration["ServiceAuth:Orchestrator:ApiKey"]
            ?? throw new InvalidOperationException("ServiceAuth:Orchestrator:ApiKey is not configured.");

        services.AddHttpClient("Orchestrator", client =>
        {
            client.BaseAddress = new Uri("https+http://orchestrator");
        })
        .AddHttpMessageHandler(() => new ApiKeyDelegatingHandler(
            Options.Create(new ApiKeyDelegatingOptions { ApiKey = apiKey })));
        return services;
    }

    public static IEndpointRouteBuilder MapOrchestratorProxy(this IEndpointRouteBuilder endpoints)
    {
        endpoints.Map("/api/orchestrator/{**path}", async (HttpContext context, IHttpClientFactory clientFactory) =>
        {
            var path = context.Request.RouteValues["path"]?.ToString() ?? "";
            var client = clientFactory.CreateClient("Orchestrator");

            var targetUri = new Uri(client.BaseAddress!, $"/api/{path}{context.Request.QueryString}");

            using var requestMessage = new HttpRequestMessage
            {
                Method = new HttpMethod(context.Request.Method),
                RequestUri = targetUri,
            };

            if (context.Request.ContentLength > 0 || context.Request.ContentType is not null)
            {
                requestMessage.Content = new StreamContent(context.Request.Body);
                if (context.Request.ContentType is not null)
                    requestMessage.Content.Headers.ContentType =
                        System.Net.Http.Headers.MediaTypeHeaderValue.Parse(context.Request.ContentType);
            }

            using var responseMessage = await client.SendAsync(requestMessage, context.RequestAborted);

            context.Response.StatusCode = (int)responseMessage.StatusCode;

            foreach (var header in responseMessage.Content.Headers)
                context.Response.Headers[header.Key] = header.Value.ToArray();

            context.Response.Headers.Remove("transfer-encoding");

            if (responseMessage.Content.Headers.ContentLength != 0 &&
                context.Response.StatusCode != StatusCodes.Status204NoContent)
            {
                await responseMessage.Content.CopyToAsync(context.Response.Body, context.RequestAborted);
            }
        }).RequireAuthorization();

        return endpoints;
    }
}
