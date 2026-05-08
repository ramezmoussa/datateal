using DuckHouse.Auth.ApiKey;
using DuckHouse.Ui.Server.Application.Ai;
using DuckHouse.Ui.Server.Core.Catalogs;
using DuckHouse.Ui.Server.Core.Repositories;
using DuckHouse.Ui.Server.Infrastructure.Ai;
using DuckHouse.Ui.Server.Infrastructure.Catalogs;
using DuckHouse.Ui.Server.Infrastructure.Data;
using DuckHouse.Ui.Server.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DuckHouse.Ui.Server.Infrastructure;

public static class ServiceExtensions
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var baseAddress = configuration["ControlPlane:BaseAddress"]
            ?? throw new InvalidOperationException("ControlPlane:BaseAddress is not configured.");

        var controlPlaneApiKey = configuration["ServiceAuth:ControlPlane:ApiKey"]
            ?? throw new InvalidOperationException("ServiceAuth:ControlPlane:ApiKey is not configured.");

        services.AddHttpClient<INodeRepository, NodeRepository>(
            client => client.BaseAddress = new Uri(baseAddress))
            .AddHttpMessageHandler(() => new ApiKeyDelegatingHandler(
                Options.Create(new ApiKeyDelegatingOptions { ApiKey = controlPlaneApiKey })));
        services.AddHttpClient<IKernelRepository, KernelRepository>(
            client => client.BaseAddress = new Uri(baseAddress))
            .AddHttpMessageHandler(() => new ApiKeyDelegatingHandler(
                Options.Create(new ApiKeyDelegatingOptions { ApiKey = controlPlaneApiKey })));

        services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
        services.AddScoped<IWheelPackageRepository, WheelPackageRepository>();
        services.AddScoped<IEnvironmentRepository, EnvironmentRepository>();
        services.AddScoped<ICatalogRepository, CatalogRepository>();
        services.AddScoped<ICatalogAccessService, CatalogAccessService>();
        services.AddScoped<ICatalogDatabaseService, CatalogDatabaseService>();
        services.AddScoped<ICatalogMetadataService, CatalogMetadataService>();
        services.AddScoped<IInteractivePoolRepository, InteractivePoolRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IAiProviderFactory, AiProviderFactory>();
    }
}