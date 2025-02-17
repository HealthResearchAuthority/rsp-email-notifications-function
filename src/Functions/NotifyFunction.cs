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
        // parse incoming message into the EmailNotificationMessage object
        var messageBody = message.Body.ToString();

        logger.LogAsInformation("Sending email...");

        var messageJson = JsonConvert.DeserializeObject<EmailNotificationMessage>(messageBody);

        // send the email via the Gov UK notification service
        var response = await rspNotifyService.SendEmail(messageJson);

        if (response != null)
        {
            // Complete the message which removes the message from the queue since it has been proccessed
            await messageActions.CompleteMessageAsync(message);
        }

        var parameters = $"EventName: {messageJson.EventName}, RecipientAddress: {messageJson.RecipientAddress}, TemplateId: {messageJson.EmailTemplateId}";

        logger.LogAsInformation(parameters: parameters, "Email successfully sent.");
    }
}