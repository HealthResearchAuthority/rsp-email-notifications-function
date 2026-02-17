using Notify.Interfaces;
using Notify.Models;
using Notify.Models.Responses;
using Rsp.NotifyFunction.Application.Contracts;
using Rsp.NotifyFunction.Application.DTO;

namespace Rsp.NotifyFunction.Services;

public class NotifyService(IAsyncNotificationClient notifyClient) : INotifyService
{
    public async Task<Notification> GetNotificationStatus(string notificationId)
    {
        return await notifyClient.GetNotificationByIdAsync(notificationId);
    }

    public Task<EmailNotificationResponse> SendEmail(EmailNotificationMessage emailNotificationMessage)
    {
        return notifyClient
            .SendEmailAsync(emailNotificationMessage.RecipientAdress,
                emailNotificationMessage.EmailTemplateId,
                emailNotificationMessage.Data.ToDictionary());
    }
}