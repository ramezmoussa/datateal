using DuckHouse.Core.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace DuckHouse.Orchestrator.Application;

public static class ServiceExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationServices()
        {
            services.AddMediator<ScanEntryPoint>();
            return services;
        }
    }
}

file class ScanEntryPoint;
