using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rsp.NotifyFunction.Client;
using Rsp.NotifyFunction.Configuration;
using Rsp.NotifyFunction.Contracts;
using Rsp.NotifyFunction.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddServices();

        services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 3;
            });
        });
    })
    .Build();

host.Run();