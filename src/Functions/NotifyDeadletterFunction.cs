namespace Rsp.NotifyFunction.Functions;

public class NotifyDeadletterFunction(
    ServiceBusClient serviceBusClient,
    IOptions<AppSettings> appSettings,
    ILogger<NotifyDeadletterFunction> logger,
    IEmailNotificationService emailNotificationService)
{
    private const int DefaultMaxRetries = 5;
    private readonly AppSettings _appSettings = appSettings.Value;

    [Function(nameof(NotifyDeadletterFunction))]
    public async Task Run(
        [ServiceBusTrigger(
            "%DeadletterQueueName%",
            Connection = "EmailNotificationServiceBus",
            AutoCompleteMessages = false)]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        var retryCount = GetRetryCount(message);
        var maxRetries = GetMaxRetries();
        var body = message.Body;

        

        EmailEnvelope? envelope = JsonSerializer.Deserialize<EmailEnvelope>(body);


        if (retryCount >= maxRetries)
        {
            logger.LogError(
                "Permanent failure after {RetryCount} DLQ retries. MessageId: {MessageId}, Reason: {Reason}, Description: {Description}, Body: {Body}",
                retryCount,
                message.MessageId,
                message.DeadLetterReason,
                message.DeadLetterErrorDescription,
                body.ToString());

            await messageActions.CompleteMessageAsync(message);

            await emailNotificationService.Error(new EmailNotificationDto()
            {
                Id = envelope.EmailNotificationId,
                Status = EmailNotificationStatuses.Sent,
                FailedAt = DateTime.Now,
                ErrorMessage = $"{message.DeadLetterReason} {message.DeadLetterErrorDescription}"
            });
            return;
        }

        var queueName = Environment.GetEnvironmentVariable("QueueName");

        if (string.IsNullOrWhiteSpace(queueName))
        {
            logger.LogError(
                "QueueName is not configured. Unable to requeue message. MessageId: {MessageId}",
                message.MessageId);

            await messageActions.AbandonMessageAsync(message);
            return;
        }

        await using var sender = serviceBusClient.CreateSender(queueName);

        var newMessage = new ServiceBusMessage(body)
        {
            ContentType = message.ContentType,
            CorrelationId = message.CorrelationId,
            Subject = message.Subject,
            MessageId = Guid.NewGuid().ToString()
        };

        foreach (var prop in message.ApplicationProperties)
        {
            newMessage.ApplicationProperties[prop.Key] = prop.Value;
        }

        newMessage.ApplicationProperties["DlqRetryCount"] = retryCount + 1;

        if (!newMessage.ApplicationProperties.ContainsKey("OriginalMessageId"))
        {
            newMessage.ApplicationProperties["OriginalMessageId"] = message.MessageId;
        }

        await sender.SendMessageAsync(newMessage);

        logger.LogInformation(
            "Requeued message from DLQ. OriginalMessageId: {OriginalMessageId}, RetryCount: {RetryCount}, NewMessageId: {NewMessageId}",
            newMessage.ApplicationProperties["OriginalMessageId"],
            retryCount + 1,
            newMessage.MessageId);

        await messageActions.CompleteMessageAsync(message);
    }

    private int GetMaxRetries()
    {
        return appSettings.Value.DlqMaxRetries > 0
            ? appSettings.Value.DlqMaxRetries
            : DefaultMaxRetries;
    }

    private static int GetRetryCount(ServiceBusReceivedMessage message)
    {
        if (message.ApplicationProperties.TryGetValue("DlqRetryCount", out var value))
        {
            if (value is int intValue)
            {
                return intValue;
            }

            if (int.TryParse(value?.ToString(), out var parsed))
            {
                return parsed;
            }
        }

        return 0;
    }
}