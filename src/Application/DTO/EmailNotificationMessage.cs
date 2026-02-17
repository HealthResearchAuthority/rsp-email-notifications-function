namespace Rsp.NotifyFunction.Application.DTO;

/// <summary>
/// Represents the data required to send an email notification used by the send email service,
/// including the event type, email template ID,
/// </summary>
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