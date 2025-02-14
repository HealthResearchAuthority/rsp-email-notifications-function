﻿using Microsoft.Extensions.Logging;
using Notify.Client;
using Notify.Models.Responses;
using Rsp.Logging.Extensions;
using Rsp.NotifyFunction.Contracts;
using Rsp.NotifyFunction.Models;

namespace Rsp.NotifyFunction.Services
{
    public class RSPNotifyService(IRSPNotifyClient notifyClient, ILogger<RSPNotifyService> logger) : IRSPNotifyService
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