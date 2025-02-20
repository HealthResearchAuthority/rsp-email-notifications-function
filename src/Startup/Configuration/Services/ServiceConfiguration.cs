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
<<<<<<<< HEAD:src/Startup/Configuration/ServiceConfiguration.cs
        services.AddTransient<IAsyncNotificationClient, NotificationClient>(sp =>
        {
            return new NotificationClient(appSettings.GovNotifyApiKey);
        });
========
        services.AddTransient<IAsyncNotificationClient, NotificationClient>
        (_ =>
            new NotificationClient(appSettings.GovNotifyApiKey)
        );
>>>>>>>> 2af55c71dc58dfb5e227c0bd0330e88b19a60395:src/Startup/Configuration/Services/ServiceConfiguration.cs

        return services;
    }
}