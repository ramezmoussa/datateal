using DuckHouse.Ui.Server.Core.Repositories;
using DuckHouse.Ui.Server.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DuckHouse.Ui.Server.Infrastructure;

public static class ServiceExtensions
{
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddHttpClient<INodeRepository, NodeRepository>(
            static client => client.BaseAddress = new Uri("http://control-plane"));
        services.AddHttpClient<IKernelRepository, KernelRepository>(
            static client => client.BaseAddress = new Uri("http://control-plane"));
    }
}