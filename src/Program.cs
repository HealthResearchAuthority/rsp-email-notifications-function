using System.Configuration;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Rsp.Logging.Extensions;
using Rsp.Logging.Interceptors;
using Rsp.NotifyFunction.Application.Configuration;
using Rsp.NotifyFunction.Application.Constants;
using Rsp.NotifyFunction.Infrastructure;
using Rsp.NotifyFunction.Startup.Configuration;

var builder = FunctionsApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var featureManager = new FeatureManager(new ConfigurationFeatureDefinitionProvider(configuration));

builder.Configuration.AddJsonFile("local.settings.json");
builder.Configuration.AddUserSecrets<Program>();
var appSettingsSection = builder.Configuration.GetSection(nameof(AppSettings));
var appSettings = appSettingsSection.Get<AppSettings>()!;


services.AddServices(appSettings);

services.ConfigureHttpClientDefaults(http =>
{
    // Turn on resilience by default
    http.AddStandardResilienceHandler(options =>
    {
        options.Retry.MaxRetryAttempts = 3;
    });
});

if (await featureManager.IsEnabledAsync(Features.InterceptedLogging))
{
    services.AddLoggingInterceptor<LoggingInterceptor>();
}
if (builder.Environment.IsDevelopment())
{
    // Load configuration from Azure App Configuration
    builder.Configuration.AddAzureAppConfiguration(
        options =>
        {
            options.Connect
            (
                new Uri(appSettings!.AzureAppConfiguration.Endpoint),
                new ManagedIdentityCredential(appSettings.AzureAppConfiguration.IdentityClientId)
            )
            .Select(KeyFilter.Any)
            .Select(KeyFilter.Any, AppSettings.ServiceLabel)
            .ConfigureRefresh(refreshOptions =>
                refreshOptions
                .Register("AppSettings:Sentinel", AppSettings.ServiceLabel, refreshAll: true)
                .SetRefreshInterval(new TimeSpan(0, 0, 15))
            );
        }
    );

    services.AddAzureAppConfiguration();
}

builder.UseMiddleware<ExceptionHandlingMiddleware>();
var host = builder.Build();
await host.RunAsync();