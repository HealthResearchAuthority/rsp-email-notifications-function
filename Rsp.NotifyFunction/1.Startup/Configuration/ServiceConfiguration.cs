using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Rsp.NotifyFunction.Application.Contracts;
using Rsp.NotifyFunction.Client;
using Rsp.NotifyFunction.Services;

namespace Rsp.NotifyFunction.Startup.Configuration
{
    public static class ServicesConfiguration
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetryWorkerService();
            services.ConfigureFunctionsApplicationInsights();
            services.AddTransient<INotifyService, NotifyService>();
            services.AddTransient<INotifyClient, NotifyClient>();

            return services;
        }
    }
}