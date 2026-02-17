using Azure.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Rsp.NotifyFunction.Application.Configuration;
using Rsp.NotifyFunction.Functions;

namespace Rsp.NotifyFunction.Infrastructure.HttpMessageHandlers;

public class AuthHeadersHandler(TokenCredential credential, ILogger<EmailNotificationFunction> logger, AppSettings appSettings) : DelegatingHandler
{
    private readonly ILogger<EmailNotificationFunction> _logger = logger;

    /// <summary>Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.</summary>
    /// <param name="request">The HTTP request message to send to the server.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> was <see langword="null" />.</exception>
    /// <returns>The task object representing the asynchronous operation.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start: Fetch Token from Managed Identity to call IRAS Service");

        var accessToken = await credential.GetTokenAsync(
                new TokenRequestContext([appSettings.ApplicationServiceApplicationId]), cancellationToken);
        request.Headers.Remove(HeaderNames.Authorization);
        request.Headers.Add(HeaderNames.Authorization, $"Bearer {accessToken.Token}");
        Console.WriteLine(accessToken.Token);

        _logger.LogInformation("End: Fetch Token from Managed Identity to call IRAS Service");

        // Use the token to make the call.
        return await base.SendAsync(request, cancellationToken);
    }
}