namespace Rsp.NotifyFunction.UnitTests.Application.EventRouters;

public class EmailHandlerRouterTests
{
    [Fact]
    public async Task Route_WhenMatchingHandlerExists_CallsHandle()
    {
        // Arrange
        var envelope = CreateEnvelope(NotificationTypes.SponsorAdded);

        var handler = new Mock<IEmailHandler>();
        handler.SetupGet(x => x.EventType).Returns(NotificationTypes.SponsorAdded);

        var sut = new EmailHandlerRouter([handler.Object]);

        // Act
        await sut.Route(envelope);

        // Assert
        handler.Verify(x => x.Handle(It.Is<EmailEnvelope>(e =>
                e.EventType == envelope.EventType &&
                e.EmailTemplateId == envelope.EmailTemplateId)),
            Times.Once);
    }

    [Fact]
    public async Task Route_WhenNoMatchingHandlerExists_DoesNotThrow()
    {
        // Arrange
        var envelope = CreateEnvelope("UNKNOWN_EVENT");

        var handler = new Mock<IEmailHandler>();
        handler.SetupGet(x => x.EventType).Returns(NotificationTypes.SponsorAdded);

        var sut = new EmailHandlerRouter([handler.Object]);

        // Act
        var act = async () => await sut.Route(envelope);

        // Assert
        await act.ShouldNotThrowAsync();

        handler.Verify(x => x.Handle(It.IsAny<EmailEnvelope>()), Times.Never);
    }

    [Fact]
    public async Task Route_WhenMultipleHandlersExist_CallsOnlyMatchingHandler()
    {
        // Arrange
        var envelope = CreateEnvelope(NotificationTypes.ModAuth);

        var sponsorAddedHandler = new Mock<IEmailHandler>();
        sponsorAddedHandler.SetupGet(x => x.EventType).Returns(NotificationTypes.SponsorAdded);

        var modAuthHandler = new Mock<IEmailHandler>();
        modAuthHandler.SetupGet(x => x.EventType).Returns(NotificationTypes.ModAuth);

        var sut = new EmailHandlerRouter([sponsorAddedHandler.Object, modAuthHandler.Object]);

        // Act
        await sut.Route(envelope);

        // Assert
        modAuthHandler.Verify(x => x.Handle(It.Is<EmailEnvelope>(e =>
                e.EventType == NotificationTypes.ModAuth &&
                e.EmailTemplateId == envelope.EmailTemplateId)),
            Times.Once);

        sponsorAddedHandler.Verify(x => x.Handle(It.IsAny<EmailEnvelope>()), Times.Never);
    }

    private static EmailEnvelope CreateEnvelope(string eventType)
    {
        return new EmailEnvelope
        {
            EventType = eventType,
            EmailTemplateId = "template-123",
            UserIdsOrEmails = ["test@example.com"],
            Data = JsonSerializer.SerializeToElement(new { Name = "Test User" })
        };
    }
}