using Microsoft.Extensions.Logging;
using Notify.Interfaces;
using Notify.Models.Responses;
using Rsp.NotifyFunction.Application.Contracts;
using Rsp.NotifyFunction.Application.Models;

namespace Rsp.NotifyFunction.Services;

public class NotifyService(IAsyncNotificationClient notifyClient, ILogger<NotifyService> logger) : INotifyService
{
    public Task<EmailNotificationResponse> SendEmail(EmailNotificationMessage emailNotificationMessage)
    {
        return notifyClient
            .SendEmailAsync(emailNotificationMessage.RecipientAddress,
                emailNotificationMessage.EmailTemplateId,
                emailNotificationMessage.Data);
    }
}