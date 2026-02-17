namespace Rsp.NotifyFunction.Application.DTO;

public class EmailNotificationMessage
{
    public string? EventType { get; set; } = null!;
    public string? EventName { get; set; } = null!;
    public string EmailTemplateId { get; set; } = null!;

    /// <summary>
    /// Single email addresses of the recipient
    /// </summary>
    public string RecipientAdress { get; set; } = null!;

    /// <summary>
    /// Personalisation data for any placeholder fields in the email template
    /// </summary>
    public IDictionary<string, dynamic> Data { get; set; } = new Dictionary<string, dynamic>();
}