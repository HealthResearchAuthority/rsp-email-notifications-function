using Notify.Client;

namespace Rsp.NotifyFunction.Application.Contracts;

public interface INotifyClient
{
    NotificationClient GetClient();
}