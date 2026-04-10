namespace Rsp.NotifyFunction.Functions;

public class NotifyFunction(
    ILogger<NotifyFunction> logger,
    IEmailHandlerRouter router,
    IEmailNotificationService emailNotificationService)
{
    // Function that listens to the azure service bus queue for new messages
    // and is triggered when a new message is added to the queue
    [Function(nameof(NotifyFunction))]
    public async Task Run(
        [ServiceBusTrigger(
            "%AppSettings:EmailNotificationQueueName%",
            Connection = "AppSettings:EmailNotificationServiceBus",
            AutoCompleteMessages = false)]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        var body = message.Body.ToString();
        EmailEnvelope? envelope = null;

        try
        {
            envelope = JsonSerializer.Deserialize<EmailEnvelope>(body);

            if (envelope == null)
            {
                logger.LogWarning("Message could not be deserialized. MessageId: {MessageId}", message.MessageId);

                await messageActions.DeadLetterMessageAsync(
                    message,
                    deadLetterReason: "DeserializationFailed",
                    deadLetterErrorDescription: "Message body could not be deserialized to EmailEnvelope.");

                return;
            }

            await router.Route(envelope);

            logger.LogInformation(
                "Email successfully sent. MessageId: {MessageId}, EventType: {EventType}, TemplateId: {TemplateId}",
                message.MessageId,
                envelope.EventType,
                envelope.EmailTemplateId);

            await messageActions.CompleteMessageAsync(message);

            await emailNotificationService.Success(new EmailNotificationDto
            {
                Id = envelope.EmailNotificationId,
                Status = EmailNotificationStatuses.Sent,
                SentAt = DateTime.Now
            });
        }
        catch (NotifyClientException ex)
        {
            logger.LogError(
                ex,
                "Notify failed. Moving message to DLQ. MessageId: {MessageId}, EventType: {EventType}",
                message.MessageId,
                envelope?.EventType);

            await messageActions.DeadLetterMessageAsync(
                message,
                deadLetterReason: "NotifySendFailed",
                deadLetterErrorDescription: ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected failure. Moving message to DLQ. MessageId: {MessageId}, EventType: {EventType}",
                message.MessageId,
                envelope?.EventType);

            await messageActions.DeadLetterMessageAsync(
                message,
                deadLetterReason: "UnhandledException",
                deadLetterErrorDescription: ex.Message);
        }
    }
}