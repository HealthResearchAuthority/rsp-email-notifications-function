namespace Rsp.NotifyFunction.Application.EmailHandlers;

/// <summary>
///     Handles the PROJECT_CLOSURE email event.
///     This handler is responsible for:
///     1. Retrieving user email addresses from User Management
///     2. Extracting project closure data from the envelope
///     3. Deduplicating and sanitising those email addresses
///     4. Sending individual email notifications using the configured template
/// </summary>
public class ProjectClosureHandler(
    INotifyService notifyService,
    IUserEmailResolver userEmailResolver)
    : IEmailHandler
{
    /// <summary>
    ///     The event type this handler is responsible for.
    ///     Must match the EventType configured in EmailTemplates.
    /// </summary>
    public string EventType => NotificationTypes.ProjectClosure;

    /// <summary>
    ///     Handles the Project Closure notification workflow.
    ///     Flow:
    ///     - Fetch users by IDs from the envelope
    ///     - Deserialize template-specific data (project closure details)
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
        // Step 0: Extract template-specific data from the envelope
        // This contains values required by the email template
        // ------------------------------------------------------------
        var additionalData = envelope.Data.Deserialize<ProjectClosureDto>();

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
                     RecipientAddress = email,
                     Data = new Dictionary<string, dynamic>
                     {
                         { "iras_id", additionalData.IrasId },
                         { "short_title", additionalData.ShortProjectTitle }
                     }
                 }))
        {
            await notifyService.SendEmail(message);
        }
    }
}