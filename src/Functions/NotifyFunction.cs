using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rsp.Logging.Extensions;
using Rsp.NotifyFunction.Application.Contracts;
using Rsp.NotifyFunction.Application.Models;

namespace Rsp.NotifyFunction.Functions;

public class NotifyFunction(ILogger<NotifyFunction> logger, INotifyService rspNotifyService)
{
    // function that listens to the azure service bus queue for new messages and is triggered when a new message is added to the queue
    [Function(nameof(NotifyFunction))]
    public async Task Run
    (
        [ServiceBusTrigger("%QueueName%", Connection = "EmailNotificationsConnection")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions
    )
    {
        // convert the received message into json string
        var notificationMessage = message.Body.ToString();

        // parse incoming message into the EmailNotificationMessage object
        var emailNotification = JsonConvert.DeserializeObject<EmailNotificationMessage>(notificationMessage);

        if (emailNotification == null)
        {
            logger.LogAsWarning("Message could not be deserialized.");
            return;
        }

        logger.LogAsInformation("Sending email...");

        // send the email via the Gov UK notification service
        var response = await rspNotifyService.SendEmail(emailNotification);

        if (response != null)
        {
            // Complete the message which removes the message from the queue since it has been proccessed
            await messageActions.CompleteMessageAsync(message);
        }

        var parameters = $"EmailName: {emailNotification.EventName}, EmailAddress: {emailNotification.RecipientAddress}, EmailTemplateId: {emailNotification.EmailTemplateId}";

        logger.LogAsInformation(parameters: parameters, "Email successfully sent.");
    }
}