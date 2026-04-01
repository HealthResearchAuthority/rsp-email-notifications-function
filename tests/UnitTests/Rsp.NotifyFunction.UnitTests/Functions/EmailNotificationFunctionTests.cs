namespace Rsp.NotifyFunction.UnitTests.Functions;

public class EmailNotificationFunctionTests : TestServiceBase<EmailNotificationFunction>
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
        await Sut.Notify(message, messageActions.Object);

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
        await Sut.Notify(message, messageActions.Object);

        // Assert
        Mocker.GetMock<IEmailHandlerRouter>()
            .Verify(x => x.Route(It.IsAny<EmailEnvelope>()), Times.Never);
    }

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

internal sealed class TestHttpRequestData : HttpRequestData
{
    public TestHttpRequestData(FunctionContext functionContext, string body)
        : base(functionContext)
    {
        Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
        Headers = new HttpHeadersCollection();
        Cookies = [];
        Url = new Uri("http://localhost/api/notify");
        Identities = [];
        Method = "POST";
    }

    public override Stream Body { get; }
    public override HttpHeadersCollection Headers { get; }
    public override IReadOnlyCollection<IHttpCookie> Cookies { get; }
    public override Uri Url { get; }
    public override IEnumerable<ClaimsIdentity> Identities { get; }
    public override string Method { get; }

    public override HttpResponseData CreateResponse()
    {
        return new TestHttpResponseData(FunctionContext);
    }
}

internal sealed class TestHttpResponseData : HttpResponseData
{
    public TestHttpResponseData(FunctionContext functionContext)
        : base(functionContext)
    {
        Body = new MemoryStream();
        Headers = new HttpHeadersCollection();
        StatusCode = HttpStatusCode.OK;
    }

    public override HttpStatusCode StatusCode { get; set; }
    public override HttpHeadersCollection Headers { get; set; }
    public override Stream Body { get; set; }
    public override HttpCookies Cookies { get; }
}