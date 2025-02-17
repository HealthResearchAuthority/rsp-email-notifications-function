using System.Text;
using Newtonsoft.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rsp.Logging.Extensions;
using Rsp.NotifyFunction.Application.Contracts;
using Rsp.NotifyFunction.Application.Models;

namespace Rsp.NotifyFunction.Functions;

public class NotifyFunction
{
    private readonly ILogger<NotifyFunction> _logger;
    private readonly INotifyService _rspNotifyService;

    public NotifyFunction(IConfiguration configuration, ILogger<NotifyFunction> logger, INotifyService rspNotifyService)
    {
        _logger = logger;
        _rspNotifyService = rspNotifyService;
    }

    // function that listens to the azure service bus queue for new messages and is triggered when a new message is added to the queue
    [Function(nameof(NotifyFunction))]
    public async Task Run(
        [ServiceBusTrigger("%QueueName%", Connection = "EmailNotificationsConnection")]
            ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        // parse incoming message into the EmailNotificationMessage object
        string messageBody = message.Body.ToString();
        _logger.LogAsInformation($"Sending email... {messageBody}");
        EmailNotificationMessage? messageJson = JsonConvert.DeserializeObject<EmailNotificationMessage>(messageBody);

        // send the email via the Gov UK notification service
        var response = await _rspNotifyService.SendEmail(messageJson);
        if (response != null)
        {
            // Complete the message which removes the message from the queue since it has been proccessed
            await messageActions.CompleteMessageAsync(message);
        }
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("Email successfully sent. Message Notification Type: {0}, Message Email Address: {1}, Message Template Id: {2}.", messageJson.EventName, messageJson.RecipientAdress, messageJson.EmailTemplateId);
        _logger.LogAsInformation(sb.ToString());
    }
}