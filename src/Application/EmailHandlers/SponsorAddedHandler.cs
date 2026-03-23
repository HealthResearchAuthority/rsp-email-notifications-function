namespace Rsp.NotifyFunction.Application.EmailHandlers;

/// <summary>
/// Handles the SPONSOR_ADDED email event.
/// 
/// This handler is responsible for:
/// 1. Retrieving user email addresses from User Management
/// 2. Deduplicating and sanitising those email addresses
/// 3. Sending individual email notifications using the configured template
/// </summary>
public class SponsorAddedHandler(
    INotifyService notifyService,
    IUserManagementServiceClient userManagementClient)
    : IEmailHandler
{
    /// <summary>
    /// The event type this handler is responsible for.
    /// Must match the EventType configured in EmailTemplates.
    /// </summary>
    public string EventType => "SPONSOR_ADDED";

    /// <summary>
    /// Handles the Sponsor Added notification workflow.
    /// 
    /// Flow:
    /// - Fetch users by IDs from the envelope
    /// - Extract and clean email addresses
    /// - Remove duplicates
    /// - Send one email per recipient (Notify does not support bulk send)
    /// </summary>
    public async Task Handle(EmailEnvelope envelope)
    {
        // Defensive: ensure we actually have user IDs to process
        if (!envelope.UserIds.Any())
        {
            return;
        }

        var emails = new List<string>();

        // ------------------------------------------------------------
        // Step 1: Retrieve user details from User Management service
        // ------------------------------------------------------------
        var usersResponse = await userManagementClient.GetUsersById(
            envelope.UserIds,
            pageIndex: 1,
            pageSize: 1000);

        if (usersResponse is { IsSuccessStatusCode: true, Content: not null })
        {
            // Extract email addresses from returned users
            emails.AddRange(usersResponse.Content.Users.Select(user => user.Email));
        }

        // ------------------------------------------------------------
        // Step 2: Clean and deduplicate email addresses
        // - Remove null/empty
        // - Trim whitespace
        // - Normalize casing
        // - Ensure uniqueness
        // ------------------------------------------------------------
        var uniqueEmails = emails
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .Select(e => e.Trim().ToLowerInvariant())
            .Distinct()
            .ToList();

        // ------------------------------------------------------------
        // Step 3: Send email notifications
        // Note:
        // Notify service does NOT support bulk sending,
        // so we must send one request per recipient.
        // ------------------------------------------------------------
        foreach (var message in uniqueEmails.Select(email => new EmailNotificationMessage
                 {
                     EmailTemplateId = envelope.EmailTemplateId, // Template tied to SPONSOR_ADDED
                     EventType = envelope.EventType,             // Should be "SPONSOR_ADDED"
                     RecipientAddress = email,
                 }))
        {
            await notifyService.SendEmail(message);
        }
    }
}