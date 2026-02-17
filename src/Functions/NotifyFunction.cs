using System.Text.Json;
using Azure.Messaging.ServiceBus;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Rsp.Logging.Extensions;
using Rsp.NotifyFunction.Application.Contracts;
using Rsp.NotifyFunction.Application.Models.CQRS;

namespace Rsp.NotifyFunction.Functions;

public class NotifyFunction(
    ILogger<NotifyFunction> logger,
    IEmailRequestFactory eventFactory,
    IMediator mediator)
{
    // function that listens to the azure service bus queue for new messages and is triggered when a new message is added to the queue
    [Function(nameof(NotifyFunction))]
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

        var evt = eventFactory.Create(envelope);

        if (evt == null)
        {
            logger.LogInformation(
                "Ignoring unsupported event type {EventType}",
                envelope.EventType);
            return;
        }

        // send the email via the Gov UK notification service
        logger.LogAsInformation("Sending email...");

        await mediator.Send(evt);

        var parameters = $"EventData: {envelope.Data}, EventType: {envelope.EventType}, TemplateId: {envelope.EmailTemplateId}";

        logger.LogAsInformation(parameters: parameters, "Email successfully sent.");
    }

    [Function("NotifyFunctionManual")]
    public async Task NotifyManual(
       [HttpTrigger(AuthorizationLevel.Function, "post", Route = "notify")]
        HttpRequestData req,
        ServiceBusReceivedMessage message)
    {
        logger.LogInformation("Received HTTP notification request.");

        var body = await new StreamReader(req.Body).ReadToEndAsync();

        var envelope = JsonSerializer.Deserialize<EmailEnvelope>(body);

        if (envelope == null)
        {
            logger.LogAsWarning("Message could not be deserialized.");
            return;
        }

        var evt = eventFactory.Create(envelope);

        if (evt == null)
        {
            logger.LogInformation(
                "Ignoring unsupported event type {EventType}",
                envelope.EventType);
            return;
        }

        // send the email via the Gov UK notification service
        logger.LogAsInformation("Sending email...");

        await mediator.Send(evt);

        var parameters = $"EventData: {envelope.Data}, EventType: {envelope.EventType}, TemplateId: {envelope.EmailTemplateId}";

        logger.LogAsInformation(parameters: parameters, "Email successfully sent.");
    }
}