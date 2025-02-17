using Microsoft.Extensions.Logging;
using Notify.Client;
using Notify.Models.Responses;
using Rsp.Logging.Extensions;
using Rsp.NotifyFunction.Application.Contracts;
using Rsp.NotifyFunction.Application.Models;

namespace Rsp.NotifyFunction.Services
{
    public class NotifyService(INotifyClient notifyClient, ILogger<NotifyService> logger) : INotifyService
    {
        private readonly NotificationClient govUkNotifyClient = notifyClient.GetClient();

        public bool SendEmail(EmailNotificationMessage emailNotificationMessage)
        {
            var result = false;
            try
            {
                EmailNotificationResponse response = govUkNotifyClient
                    .SendEmail(emailNotificationMessage.RecipientAdress,
                        emailNotificationMessage.EmailTemplateId,
                        emailNotificationMessage.Data);

                //TODO potentially add a callback to check the status of the email sent i.e. sending, delivered etc

                // check for response object to see if the email has been sent
                if (response != null)
                {
                    result = true;
                }
            }
            catch (Notify.Exceptions.NotifyClientException ex)
            {
                logger.LogAsError("GOV_UK_NOTIFY_CLIENT_EXCEPTION", ex.Message, ex);
                result = false;
            }

            return result;
        }
    }
}