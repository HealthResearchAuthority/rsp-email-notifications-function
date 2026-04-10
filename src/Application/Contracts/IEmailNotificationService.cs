namespace Rsp.NotifyFunction.Application.Contracts;

public interface IEmailNotificationService
{
    Task Success(EmailNotificationDto emailNotificationRequest);
    Task Error(EmailNotificationDto emailNotificationRequest);
}