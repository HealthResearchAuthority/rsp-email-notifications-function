using Rsp.NotifyFunction.Application.EmailHandlers;
using Rsp.NotifyFunction.Application.EventRouters;

namespace Rsp.NotifyFunction.Startup.Configuration;

public static class ServicesConfiguration
{
    public static IServiceCollection AddServices(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddTransient<INotifyService, NotifyService>();

        services.AddTransient<IAsyncNotificationClient, NotificationClient>(_ =>
            new NotificationClient(appSettings.GovNotifyApiKey));

        services.AddScoped<IEmailHandlerRouter, EmailHandlerRouter>();
        services.AddScoped<IUserEmailResolver, UserEmailResolver>();

        // ADD EMAIL HANDLERS
        services.AddScoped<IEmailHandler, SponsorAddedHandler>();
        services.AddScoped<IEmailHandler, RevisionRequestedHandler>();
        services.AddScoped<IEmailHandler, ModificationAuthorisationHandler>();
        services.AddScoped<IEmailHandler, ProjectClosureHandler>();
        services.AddScoped<IEmailHandler, ModificationOutcomeHandler>();
        services.AddScoped<IEmailHandler, FurtherInformationHandler>();

        return services;
    }
}