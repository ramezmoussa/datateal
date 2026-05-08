using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Application.Ai;
using Microsoft.Extensions.DependencyInjection;

namespace DuckHouse.Ui.Server.Application;

public static class ServiceExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationServices()
        {
            services.AddMediator<ScanEntryPoint>();
            services.AddScoped<IAiChatService, AiChatService>();
            services.AddScoped<IContextAssembler, ContextAssembler>();
            return services;
        }
    }
}

file class ScanEntryPoint;