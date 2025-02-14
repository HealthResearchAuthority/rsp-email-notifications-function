using Rsp.NotifyFunction.Models;

namespace Rsp.NotifyFunction.Contracts
{
    public interface IRSPNotifyService
    {
        bool SendEmail(EmailNotificationMessage emailNotificationMessage);
    }
}