using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Notify.Client;
using Notify.Interfaces;
using Rsp.NotifyFunction.Application.Configuration;
using Rsp.NotifyFunction.Application.Contracts;
using Rsp.NotifyFunction.Services;

namespace Rsp.NotifyFunction.Startup.Configuration.Services;

public static class ServicesConfiguration
{
    public static IServiceCollection AddServices(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddTransient<INotifyService, NotifyService>();

        services.AddTransient<IAsyncNotificationClient, NotificationClient>(_ => new NotificationClient(appSettings.GovNotifyApiKey));
        services.AddTransient<IAsyncNotificationClient, NotificationClient>();

        return services;
    }
}