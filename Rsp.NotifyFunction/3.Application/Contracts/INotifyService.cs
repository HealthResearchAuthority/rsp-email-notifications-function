using Rsp.NotifyFunction.Application.Models;

namespace Rsp.NotifyFunction.Application.Contracts
{
    public interface INotifyService
    {
        bool SendEmail(EmailNotificationMessage emailNotificationMessage);
    }
}