using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Rsp.Logging.Extensions;
using Rsp.NotifyFunction.Application.Contracts;
using Rsp.NotifyFunction.Application.DTO;

namespace Rsp.NotifyFunction.Functions;

public class EmailNotificationFunction(
    ILogger<EmailNotificationFunction> logger,
    IEmailHandlerRouter router)
{
    // Function that listens to the azure service bus queue for new messages
    // and is triggered when a new message is added to the queue
    [Function(nameof(EmailNotificationFunction))]
    public async Task Notify
    (
        [ServiceBusTrigger("%QueueName%", Connection = "EmailNotificationsConnection")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions
    )
    {
        // convert the received message into json string
        var notificationMessage = message.Body.ToString();
        var envelope = JsonSerializer.Deserialize<EmailEnvelope>(notificationMessage);

        if (envelope == null)
        {
            logger.LogAsWarning("Message could not be deserialized.");
            return;
        }

        logger.LogAsInformation("Sending email...");

        // send the email via the Gov UK notification service
        await router.Route(envelope);

        var parameters = $"EventData: {envelope.Data}, EventType: {envelope.EventType}, TemplateId: {envelope.EmailTemplateId}";

        // log the details of the email that was sent, including the event data, event type and email template id
        logger.LogAsInformation(parameters: parameters, "Email successfully sent.");
    }

    // This is an additional HTTP triggered function that can
    // be used for manual testing and sending of email notifications.
    [Function("NotifyFunctionManual")]
    public async Task NotifyManual(
       [HttpTrigger(AuthorizationLevel.Function, "post", Route = "notify")]
        HttpRequestData req,
        ServiceBusReceivedMessage message)
    {
        logger.LogInformation("Received HTTP notification request.");

        // convert the received message into json string
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        var envelope = JsonSerializer.Deserialize<EmailEnvelope>(body);

        if (envelope == null)
        {
            logger.LogAsWarning("Message could not be deserialized.");
            return;
        }

        logger.LogAsInformation("Sending email...");

        // send the email via the Gov UK notification service
        await router.Route(envelope);

        var parameters = $"EventData: {envelope.Data}, EventType: {envelope.EventType}, TemplateId: {envelope.EmailTemplateId}";

        // log the details of the email that was sent
        logger.LogAsInformation(parameters: parameters, "Email successfully sent.");
    }
}