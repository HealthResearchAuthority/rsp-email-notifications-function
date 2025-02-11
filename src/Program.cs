using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Rsp.Logging.Extensions;
using Rsp.Logging.Interceptors;
using Rsp.NotifyFunction.Application.Configuration;
using Rsp.NotifyFunction.Application.Constants;
using Rsp.NotifyFunction.Infrastructure;
using Rsp.NotifyFunction.Startup.Configuration.AppConfiguration;
using Rsp.NotifyFunction.Startup.Configuration.Services;

var builder = FunctionsApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

configuration
    .AddJsonFile("local.settings.json", true)
    .AddJsonFile("featuresettings.json", true, true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

var appSettingsSection = builder.Configuration.GetSection(nameof(AppSettings));
var appSettings = appSettingsSection.Get<AppSettings>()!;

services.AddServices(appSettings);

services.ConfigureHttpClientDefaults(http =>
{
    // Turn on resilience by default
    http.AddStandardResilienceHandler(options => options.Retry.MaxRetryAttempts = 3);
});

if (!builder.Environment.IsDevelopment())
{
    // Load configuration from Azure App Configuration
    services.AddAzureAppConfiguration(configuration);
}

// Creating a feature manager without the use of DI. Injecting IFeatureManager
// via DI is appropriate in consturctor methods. At the startup, it's
// not recommended to call services.BuildServiceProvider and retreive IFeatureManager
// via provider. Instead, the follwing approach is recommended by creating FeatureManager
// with ConfigurationFeatureDefinitionProvider using the existing configuration.
var featureManager = new FeatureManager(new ConfigurationFeatureDefinitionProvider(configuration));

if (await featureManager.IsEnabledAsync(Features.InterceptedLogging))
{
    services.AddLoggingInterceptor<LoggingInterceptor>();
}

builder.UseMiddleware<ExceptionHandlingMiddleware>();

var host = builder.Build();

await host.RunAsync();