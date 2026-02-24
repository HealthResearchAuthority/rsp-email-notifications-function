namespace Rsp.NotifyFunction.Application.Configuration;

public class AppSettings
{
    public const string ServiceLabel = "EmailNotificationFunction";
    public AzureAppConfiguration AzureAppConfiguration { get; set; } = null!;

    public string GovNotifyApiKey { get; set; } = null!;
    public string QueueName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the URI of the ApplicationsService microservice.
    /// </summary>
    public Uri ApplicationsServiceUri { get; set; } = null!;

    /// <summary>
    /// Gets or sets application ID of the App Registeration for the Applications Service in Micorosoft Entra ID. Format: api://xxxxxxxxxxxxxxx
    /// </summary>
    public string ApplicationServiceApplicationId { get; set; } = null!;

    /// <summary>
    /// Gets or sets Managed Identity Client ID to enabling the framework to fetch a token for accessing Applications Service.
    /// </summary>
    public string ManagedIdentityClientID { get; set; } = null!;
}