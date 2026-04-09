namespace Rsp.NotifyFunction.Services;

public class EmailNotificationService(IEmailNotificationServiceClient emailNotificationServiceClient)
    : IEmailNotificationService
{
    public async Task Success(EmailNotificationDto emailNotificationRequest)
    {
        await emailNotificationServiceClient.UpdateEmailNotification(new EmailNotificationDto
        {
            Id = emailNotificationRequest.Id,
            Status = EmailNotificationStatuses.Sent,
            SentAt = DateTime.UtcNow
        });
    }

    public async Task Error(EmailNotificationDto emailNotificationRequest)
    {
        await emailNotificationServiceClient.UpdateEmailNotification(new EmailNotificationDto
        {
            Id = emailNotificationRequest.Id,
            Status = EmailNotificationStatuses.Failed,
            FailedAt = DateTime.UtcNow,
            ErrorMessage = emailNotificationRequest.ErrorMessage
        });
    }
}