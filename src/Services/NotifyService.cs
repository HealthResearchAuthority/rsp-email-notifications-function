using Notify.Interfaces;
using Notify.Models;
using Notify.Models.Responses;
using Rsp.NotifyFunction.Application.Contracts;
using Rsp.NotifyFunction.Application.DTO;

namespace Rsp.NotifyFunction.Services;

public class NotifyService(IAsyncNotificationClient notifyClient) : INotifyService
{
    /// <summary>
    /// Retrieves the status of a notification by its ID using the notify client.
    /// </summary>
    /// <param name="notificationId"></param>
    /// <returns></returns>
    public async Task<Notification> GetNotificationStatus(string notificationId)
    {
        return await notifyClient.GetNotificationByIdAsync(notificationId);
    }

    /// <summary>
    /// Sends an email notification using the provided email notification message.
    /// </summary>
    /// <param name="emailNotificationMessage"></param>
    /// <returns></returns>
    public Task<EmailNotificationResponse> SendEmail(EmailNotificationMessage emailNotificationMessage)
    {
        return notifyClient
            .SendEmailAsync(emailNotificationMessage.RecipientAdress,
                emailNotificationMessage.EmailTemplateId,
                emailNotificationMessage.Data.ToDictionary());
    }
}