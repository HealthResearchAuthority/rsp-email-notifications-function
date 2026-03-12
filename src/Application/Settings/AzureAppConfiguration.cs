namespace Rsp.NotifyFunction.Application.Settings;

[ExcludeFromCodeCoverage]
public class AzureAppConfiguration
{
    public string Endpoint { get; set; } = null!;
    public string IdentityClientId { get; set; } = null!;
}