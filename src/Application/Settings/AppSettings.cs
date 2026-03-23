namespace Rsp.NotifyFunction.Application.Settings;

[ExcludeFromCodeCoverage]
public class AppSettings
{
    public const string ServiceLabel = "EmailNotificationFunction";
    public AzureAppConfiguration AzureAppConfiguration { get; set; } = null!;
    public MicrosoftEntra MicrosoftEntra { get; set; } = null!;

    public string GovNotifyApiKey { get; set; } = null!;
    public string QueueName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the URI of the ApplicationsService microservice.
    /// </summary>
    public Uri ApplicationsServiceUri { get; set; } = null!;

    /// <summary>
    /// Gets or sets the URI of the UsersService microservice.
    /// </summary>
    public Uri UsersServiceUri { get; set; } = null!;

    /// <summary>
    /// Gets or sets Managed Identity Client ID to enabling the framework to fetch a token for accessing Applications Service.
    /// </summary>
    public string ManagedIdentityNotifyClientID { get; set; } = null!;
}