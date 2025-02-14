using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Rsp.NotifyFunction.Client;
using Rsp.NotifyFunction.Contracts;
using Rsp.NotifyFunction.Services;

namespace Rsp.NotifyFunction.Configuration
{
    public static class ServicesConfiguration
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetryWorkerService();
            services.ConfigureFunctionsApplicationInsights();
            services.AddTransient<IRSPNotifyService, RSPNotifyService>();
            services.AddTransient<IRSPNotifyClient, RSPNotifyClient>();           

            return services;
        }
    }
}
