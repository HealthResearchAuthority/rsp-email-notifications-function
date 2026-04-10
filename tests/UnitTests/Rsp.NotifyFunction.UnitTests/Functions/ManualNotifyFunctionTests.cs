namespace Rsp.NotifyFunction.UnitTests.Functions;

public class ManualNotifyFunctionTests : TestServiceBase<ManualNotifyFunction>
{
    [Fact]
    public async Task NotifyManual_WhenEnvelopeIsValid_RoutesEmail()
    {
        // Arrange
        var envelope = CreateEnvelope();
        var json = JsonSerializer.Serialize(envelope);

        var context = new Mock<FunctionContext>().Object;
        var request = new TestHttpRequestData(context, json);

        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
            BinaryData.FromString(json));

        // Act
        await Sut.NotifyManual(request, message);

        // Assert
        Mocker.GetMock<IEmailHandlerRouter>()
            .Verify(x => x.Route(It.Is<EmailEnvelope>(e =>
                    e.EventType == envelope.EventType &&
                    e.EmailTemplateId == envelope.EmailTemplateId)),
                Times.Once);
    }

    [Fact]
    public async Task NotifyManual_WhenEnvelopeIsNull_LogsWarning_AndDoesNotRouteEmail()
    {
        // Arrange
        var context = new Mock<FunctionContext>().Object;
        var request = new TestHttpRequestData(context, "null");

        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
            BinaryData.FromString("null"));

        // Act
        await Sut.NotifyManual(request, message);

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