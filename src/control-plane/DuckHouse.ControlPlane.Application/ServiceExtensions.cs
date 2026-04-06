using DuckHouse.Core.Mediator;
using DuckHouse.ControlPlane.Application.InactivityEviction;
using Microsoft.Extensions.DependencyInjection;

namespace DuckHouse.ControlPlane.Application;

public static class ServiceExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationServices()
        {
            services.AddMediator<ScanEntryPoint>();
            services.AddHostedService<InactivityEvictionService>();
            return services;
        }
    }
}

file class ScanEntryPoint;
