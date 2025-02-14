using Notify.Client;

namespace Rsp.NotifyFunction.Contracts
{
    public interface IRSPNotifyClient
    {
        NotificationClient GetClient();
    }
}