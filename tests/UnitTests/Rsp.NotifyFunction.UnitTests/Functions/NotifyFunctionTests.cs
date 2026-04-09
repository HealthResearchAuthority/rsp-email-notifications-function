namespace Rsp.NotifyFunction.UnitTests.Functions;

public class NotifyFunctionFunctionTests : TestServiceBase<NotifyFunction.Functions.NotifyFunction>
{
    [Fact]
    public async Task Notify_WhenEnvelopeIsValid_RoutesEmail()
    {
        // Arrange
        var envelope = CreateEnvelope();
        var json = JsonSerializer.Serialize(envelope);

        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
            BinaryData.FromString(json));

        var messageActions = new Mock<ServiceBusMessageActions>();

        // Act
        await Sut.Run(message, messageActions.Object);

        // Assert
        Mocker.GetMock<IEmailHandlerRouter>()
            .Verify(x => x.Route(It.Is<EmailEnvelope>(e =>
                    e.EventType == envelope.EventType &&
                    e.EmailTemplateId == envelope.EmailTemplateId)),
                Times.Once);
    }

    [Fact]
    public async Task Notify_WhenEnvelopeIsNull_LogsWarning_AndDoesNotRouteEmail()
    {
        // Arrange
        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
            BinaryData.FromString("null"));

        var messageActions = new Mock<ServiceBusMessageActions>();

        // Act
        await Sut.Run(message, messageActions.Object);

        // Assert
        Mocker.GetMock<IEmailHandlerRouter>()
            .Verify(x => x.Route(It.IsAny<EmailEnvelope>()), Times.Never);
    }


    private static EmailEnvelope CreateEnvelope()
    {
        return new EmailEnvelope
        {
            EventType = "SPONSOR_ADDED",
            EmailTemplateId = "template-123",
            UserIdsOrEmails = ["test@example.com"],
            Data = JsonSerializer.SerializeToElement(new { Name = "Test User" })
        };
    }
}