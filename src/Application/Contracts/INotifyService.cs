using Notify.Models;
using Notify.Models.Responses;
using Rsp.NotifyFunction.Application.DTO;

namespace Rsp.NotifyFunction.Application.Contracts;

/// <summary>
/// Defines the contract for a notification service
/// that can send email notifications and retrieve notification statuses.
/// </summary>
public interface INotifyService
{
    Task<EmailNotificationResponse> SendEmail(EmailNotificationMessage emailNotificationMessage);

    Task<Notification> GetNotificationStatus(string notificationId);
}