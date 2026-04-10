namespace Rsp.NotifyFunction.Application.EmailHandlers;

/// <summary>
///     Handles the SPONSOR_ADDED email event.
///     This handler is responsible for:
///     1. Retrieving user email addresses from User Management
///     2. Deduplicating and sanitising those email addresses
///     3. Sending individual email notifications using the configured template
/// </summary>
public class SponsorAddedHandler(
    INotifyService notifyService,
    IUserEmailResolver userEmailResolver)
    : IEmailHandler
{
    /// <summary>
    ///     The event type this handler is responsible for.
    ///     Must match the EventType configured in EmailTemplates.
    /// </summary>
    public string EventType => NotificationTypes.SponsorAdded;

    /// <summary>
    ///     Handles the Sponsor Added notification workflow.
    ///     Flow:
    ///     - Fetch users by IDs from the envelope
    ///     - Extract and clean email addresses
    ///     - Remove duplicates
    ///     - Send one email per recipient (Notify does not support bulk send)
    /// </summary>
    public async Task Handle(EmailEnvelope envelope)
    {
        // Defensive: ensure we actually have user IDs to process
        if (!envelope.UserIdsOrEmails.Any())
        {
            return;
        }

        // ------------------------------------------------------------
        // Step 1: Retrieve user details from User Management service
        // ------------------------------------------------------------
        // Step 2: Clean and deduplicate email addresses
        // - Remove null/empty
        // - Trim whitespace
        // - Normalize casing
        // - Ensure uniqueness
        // ------------------------------------------------------------
        var emails = await userEmailResolver.ResolveEmailsAsync(envelope.UserIdsOrEmails);

        // ------------------------------------------------------------
        // Step 3: Send email notifications
        // Note:
        // Notify service does NOT support bulk sending,
        // so we must send one request per recipient.
        // ------------------------------------------------------------
        foreach (var message in emails.Select(email => new EmailNotificationMessage
                 {
                     EmailNotificationId = envelope.EmailNotificationId,
                     EmailTemplateId = envelope.EmailTemplateId,
                     EventType = envelope.EventType,
                     RecipientAddress = email
                 }))
        {
            await notifyService.SendEmail(message);
        }
    }
}