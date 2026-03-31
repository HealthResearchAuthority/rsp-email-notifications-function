namespace Rsp.NotifyFunction.Application.Settings;

[ExcludeFromCodeCoverage]
public class MicrosoftEntra
{
    public string Authority { get; set; } = null!;

    //This is IRASServiceAPI ID in Microsoft Entra ID
    public string Audience { get; set; } = null!;
}