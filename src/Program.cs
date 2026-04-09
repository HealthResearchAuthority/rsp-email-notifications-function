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

        // 2) Common config
        builder.Configuration
            .AddJsonFile("featuresettings.json", true, true)
            .AddEnvironmentVariables();

        // 3) Attach Azure App Configuration in non-Dev
        if (!builder.Environment.IsDevelopment())
        {
            builder.Services.AddAzureAppConfiguration(builder.Configuration);
        }
        else
            // Use DefaultAzureCredential in development environment
            // Need to give access to user's account on API - Or verify if this works with user's
            // identity You should have these set in your local settings or environment variables: "AZURE_CLIENT_ID","AZURE_TENANT_ID","AZURE_CLIENT_SECRET"
        {
            builder.Services.AddSingleton<TokenCredential>(new DefaultAzureCredential());
        }

        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));
        var appSettings = builder.Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>()!;

        // ✅ ONE TokenCredential registration, chosen by environment
        builder.Services.AddSingleton<TokenCredential>(_ =>
        {
            if (builder.Environment.IsDevelopment())
            {
                // Prefer local.settings.json Values first
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

                // Uses EXACTLY what's in local.settings.json (no stale machine env vars)
                return new ClientSecretCredential(tenantId, clientId, clientSecret);
            }

            // Non-dev: Managed Identity
            return new ManagedIdentityCredential(appSettings.ManagedIdentityNotifyClientID);
        });

        builder.Services.AddSingleton(appSettings);
        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler(options => options.Retry.MaxRetryAttempts = 3);
        });

        // register dependencies
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

        // Creating a feature manager without the use of DI. Injecting IFeatureManager via DI is
        // appropriate in constructor methods. At the startup, it's not recommended to call
        // services.BuildServiceProvider and retrieve IFeatureManager via provider. Instead, the
        // following approach is recommended by creating FeatureManager with
        // ConfigurationFeatureDefinitionProvider using the existing configuration.
        var featureManager = new FeatureManager(new ConfigurationFeatureDefinitionProvider(builder.Configuration));

        if (await featureManager.IsEnabledAsync(Features.InterceptedLogging))
        {
            builder.Services.AddLoggingInterceptor<LoggingInterceptor>();
        }

        var app = builder.Build();

        await app.RunAsync();
    }
}