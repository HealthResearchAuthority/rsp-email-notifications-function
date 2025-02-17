using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        var messageBody = message.Body.ToString();
        _logger.LogAsInformation($"Sending email...");
        var messageJson = JsonConvert.DeserializeObject<EmailNotificationMessage>(messageBody);

        // send the email via the Gov UK notification service
        var response = await _rspNotifyService.SendEmail(messageJson);
        if (response != null)
        {
            // Complete the message which removes the message from the queue since it has been proccessed
            await messageActions.CompleteMessageAsync(message);
        }

        var parameters = $"Email Name: {messageJson.EventName}, Email Address: {messageJson.RecipientAddress}, Email TemplateId: {messageJson.EmailTemplateId}";
        _logger.LogAsInformation(parameters: parameters, "Email successfully sent.");
    }
}