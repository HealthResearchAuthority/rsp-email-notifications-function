using Notify.Models;
using Notify.Models.Responses;
using Rsp.NotifyFunction.Application.DTO;

namespace Rsp.NotifyFunction.Application.Contracts;

public interface INotifyService
{
    Task<EmailNotificationResponse> SendEmail(EmailNotificationMessage emailNotificationMessage);

    Task<Notification> GetNotificationStatus(string notificationId);
}