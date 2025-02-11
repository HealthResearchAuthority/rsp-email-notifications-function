using Notify.Models.Responses;
using Rsp.NotifyFunction.Application.Models;

namespace Rsp.NotifyFunction.Application.Contracts;

public interface INotifyService
{
    Task<EmailNotificationResponse> SendEmail(EmailNotificationMessage emailNotificationMessage);
}