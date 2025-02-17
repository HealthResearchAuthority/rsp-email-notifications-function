namespace Rsp.NotifyFunction.Application.Configuration;

public class AppSettings
{
    public const string ServiceLabel = "EmailNotificationFunction";
    public AzureAppConfiguration AzureAppConfiguration { get; set; } = null!;

    public string GovNotifyApiKey { get; set; } = null!;
    public string QueueName { get; set; } = null!;
}