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
        [ServiceBusTrigger("queue.1", Connection = "emailNotificationsConnection")]
            ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        // parse incoming message into the EmailNotificationMessage object
        string messageBody = message.Body.ToString();
        EmailNotificationMessage? messageJson = JsonConvert.DeserializeObject<EmailNotificationMessage>(messageBody);

        try
        {
            _logger.LogAsInformation("Sending email...\n");

            // send the email via the Gov UK notification service
            var sendEmail = _rspNotifyService.SendEmail(messageJson);

            if (sendEmail)
            {
                // Complete the message which removes the message from the queue since it has been proccessed
                await messageActions.CompleteMessageAsync(message);
            }

            _logger.LogAsInformation($"Email sucesfully sent.\n Message Notification Type: {messageJson.EventName},\n Message Email Address: {messageJson.RecipientAdress}, \nMessage Template Id: {messageJson.EmailTemplateId}");
        }
        catch (Exception ex)
        {
            _logger.LogAsError("SERVER_ERROR", ex.Message, ex);
        }
    }
}