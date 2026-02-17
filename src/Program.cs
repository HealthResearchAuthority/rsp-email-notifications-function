using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Rsp.Logging.Extensions;
using Rsp.Logging.Interceptors;
using Rsp.NotifyFunction.Application.Configuration;
using Rsp.NotifyFunction.Application.Configuration.HttpClients;
using Rsp.NotifyFunction.Application.Constants;
using Rsp.NotifyFunction.Application.Contracts;
using Rsp.NotifyFunction.Application.Factories;
using Rsp.NotifyFunction.Infrastructure;
using Rsp.NotifyFunction.Infrastructure.HttpMessageHandlers;
using Rsp.NotifyFunction.Startup.Configuration.AppConfiguration;
using Rsp.NotifyFunction.Startup.Configuration.Services;

var builder = FunctionsApplication.CreateBuilder(args);
builder.ConfigureFunctionsWebApplication();

var services = builder.Services;
var configuration = builder.Configuration;

// 1) Development-only local config
if (builder.Environment.IsDevelopment())
{
    builder.Configuration
        .AddJsonFile("local.settings.json", true, true)
        .AddUserSecrets<Program>(true);
}

// 2) Common config
builder.Configuration
    .AddJsonFile("featuresettings.json", true, true)
    .AddEnvironmentVariables();

var appSettingsSection = builder.Configuration.GetSection(nameof(AppSettings));
var appSettings = appSettingsSection.Get<AppSettings>()!;

services.AddSingleton(appSettings);
services.AddServices(appSettings);

if (!builder.Environment.IsDevelopment())
{
    // Load configuration from Azure App Configuration
    services.AddAzureAppConfiguration(configuration);

    builder.Services.AddSingleton<TokenCredential>(new ManagedIdentityCredential(appSettings.ManagedIdentityClientID));
}
else
{
    builder.Services.AddSingleton<TokenCredential>(new DefaultAzureCredential());
}

services.ConfigureHttpClientDefaults(http =>
{
    // Turn on resilience by default
    http.AddStandardResilienceHandler(options => options.Retry.MaxRetryAttempts = 3);
});

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddSingleton<IEmailRequestFactory, EmailEventFactory>();

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

services.AddTransient<AuthHeadersHandler>();
services.AddHttpClients(appSettings);

var host = builder.Build();

await host.RunAsync();