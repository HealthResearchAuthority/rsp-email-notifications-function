using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;

namespace Rsp.NotifyFunction;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = FunctionsApplication.CreateBuilder(args);
        builder.ConfigureFunctionsWebApplication();

        // 1) Development-only local config
        if (builder.Environment.IsDevelopment())
        {
            builder.Configuration
                .AddJsonFile("local.settings.json", true, true)
                .AddUserSecrets<UserSecretsAnchor>(true);
        }

        builder.Configuration
            .AddJsonFile("featuresettings.json", true, true)
            .AddEnvironmentVariables();

        if (!builder.Environment.IsDevelopment())
        {
            builder.Services.AddAzureAppConfiguration(builder.Configuration);
        }

        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));
        var appSettings = builder.Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>()!;

        builder.Services.AddSingleton<TokenCredential>(_ =>
        {
            if (builder.Environment.IsDevelopment())
            {
                var clientId = builder.Configuration["Values:AZURE_CLIENT_ID"]
                               ?? builder.Configuration["AZURE_CLIENT_ID"];
                var tenantId = builder.Configuration["Values:AZURE_TENANT_ID"]
                               ?? builder.Configuration["AZURE_TENANT_ID"];
                var clientSecret = builder.Configuration["Values:AZURE_CLIENT_SECRET"]
                                   ?? builder.Configuration["AZURE_CLIENT_SECRET"];

                if (string.IsNullOrWhiteSpace(clientId) ||
                    string.IsNullOrWhiteSpace(tenantId) ||
                    string.IsNullOrWhiteSpace(clientSecret))
                {
                    throw new InvalidOperationException(
                        "Missing AZURE_CLIENT_ID / AZURE_TENANT_ID / AZURE_CLIENT_SECRET in local.settings.json (Values:...) or environment variables.");
                }

                return new ClientSecretCredential(tenantId, clientId, clientSecret);
            }

            return new ManagedIdentityCredential(appSettings.ManagedIdentityNotifyClientID);
        });

        builder.Services.AddSingleton(appSettings);

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            http.AddStandardResilienceHandler(options => options.Retry.MaxRetryAttempts = 3);
        });

        builder.Services.AddMemoryCache();
        builder.Services.AddServices(appSettings);
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddTransient<ApplicationsServiceAuthHeadersHandler>();
        builder.Services.AddTransient<UserServiceAuthHeadersHandler>();
        builder.Services.AddHttpClients(appSettings);

        // Register ServiceBusClient
        builder.Services.AddSingleton<ServiceBusClient>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();

            var connectionString =
                configuration["ConnectionStrings:EmailNotificationServiceBus"] ??
                configuration["Values:EmailNotificationServiceBus"] ??
                configuration["EmailNotificationServiceBus"];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Missing ConnectionStrings:EmailNotificationServiceBus configuration.");
            }

            return new ServiceBusClient(connectionString);
        });

        var featureManager = new FeatureManager(new ConfigurationFeatureDefinitionProvider(builder.Configuration));

        if (await featureManager.IsEnabledAsync(Features.InterceptedLogging))
        {
            builder.Services.AddLoggingInterceptor<LoggingInterceptor>();
        }

        var app = builder.Build();

        await app.RunAsync();
    }
}