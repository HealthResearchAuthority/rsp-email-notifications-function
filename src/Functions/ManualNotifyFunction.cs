namespace Rsp.NotifyFunction.Functions;

public class ManualNotifyFunction(
    ILogger<ManualNotifyFunction> logger,
    IEmailHandlerRouter router)
{
    // This is an additional HTTP triggered function that can
    // be used for manual testing and sending of email notifications.
    [Function("ManualNotifyFunction")]
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

        var parameters =
            $"EventData: {envelope.Data}, EventType: {envelope.EventType}, TemplateId: {envelope.EmailTemplateId}";

        // log the details of the email that was sent
        logger.LogAsInformation(parameters: parameters, "Email successfully sent.");
    }
}