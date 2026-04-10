using DuckHouse.Core.Mediator;
using DuckHouse.Orchestrator.Application.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace DuckHouse.Orchestrator.Application;

public static class ServiceExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationServices()
        {
            services.AddMediator<ScanEntryPoint>();

            // Engine services
            services.AddSingleton<RunDispatcher>();
            services.AddScoped<RunCoordinator>();
            services.AddScoped<TaskExecutor>();
            services.AddHostedService<SchedulerService>();
            services.AddHostedService<RecoveryService>();

            return services;
        }
    }
}

file class ScanEntryPoint;
