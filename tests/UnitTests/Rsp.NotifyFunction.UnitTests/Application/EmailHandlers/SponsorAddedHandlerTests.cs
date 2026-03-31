namespace Rsp.NotifyFunction.UnitTests.Application.EmailHandlers;

public class SponsorAddedHandlerTests : TestServiceBase<SponsorAddedHandler>
{
    [Fact]
    public void EventType_ReturnsSponsorAdded()
    {
        // Assert
        Sut.EventType.ShouldBe(NotificationTypes.SponsorAdded);
    }

    [Fact]
    public async Task Handle_WhenUserIdsOrEmailsIsEmpty_DoesNotResolveEmails_AndDoesNotSendEmails()
    {
        // Arrange
        var envelope = new EmailEnvelope
        {
            EventType = NotificationTypes.SponsorAdded,
            EmailTemplateId = "template-123",
            UserIdsOrEmails = [],
            Data = JsonSerializer.SerializeToElement(new { })
        };

        // Act
        await Sut.Handle(envelope);

        // Assert
        Mocker.GetMock<IUserEmailResolver>()
            .Verify(x => x.ResolveEmailsAsync(It.IsAny<IEnumerable<string>>()), Times.Never);

        Mocker.GetMock<INotifyService>()
            .Verify(x => x.SendEmail(It.IsAny<EmailNotificationMessage>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenResolvedEmailsExist_SendsOneEmailPerRecipient()
    {
        // Arrange
        var envelope = new EmailEnvelope
        {
            EventType = NotificationTypes.SponsorAdded,
            EmailTemplateId = "template-123",
            UserIdsOrEmails = ["user-1", "user-2"],
            Data = JsonSerializer.SerializeToElement(new { Name = "Test User" })
        };

        var resolvedEmails = new List<string>
        {
            "one@test.com",
            "two@test.com"
        };

        Mocker.GetMock<IUserEmailResolver>()
            .Setup(x => x.ResolveEmailsAsync(envelope.UserIdsOrEmails))
            .ReturnsAsync(resolvedEmails);

        // Act
        await Sut.Handle(envelope);

        // Assert
        Mocker.GetMock<IUserEmailResolver>()
            .Verify(x => x.ResolveEmailsAsync(envelope.UserIdsOrEmails), Times.Once);

        Mocker.GetMock<INotifyService>()
            .Verify(x => x.SendEmail(It.Is<EmailNotificationMessage>(m =>
                    m.EmailTemplateId == envelope.EmailTemplateId &&
                    m.EventType == envelope.EventType &&
                    m.RecipientAddress == "one@test.com")),
                Times.Once);

        Mocker.GetMock<INotifyService>()
            .Verify(x => x.SendEmail(It.Is<EmailNotificationMessage>(m =>
                    m.EmailTemplateId == envelope.EmailTemplateId &&
                    m.EventType == envelope.EventType &&
                    m.RecipientAddress == "two@test.com")),
                Times.Once);
    }

    [Fact]
    public async Task Handle_WhenResolvedEmailsIsEmpty_DoesNotSendEmails()
    {
        // Arrange
        var envelope = new EmailEnvelope
        {
            EventType = NotificationTypes.SponsorAdded,
            EmailTemplateId = "template-123",
            UserIdsOrEmails = ["user-1"],
            Data = JsonSerializer.SerializeToElement(new { })
        };

        Mocker.GetMock<IUserEmailResolver>()
            .Setup(x => x.ResolveEmailsAsync(envelope.UserIdsOrEmails))
            .ReturnsAsync([]);

        // Act
        await Sut.Handle(envelope);

        // Assert
        Mocker.GetMock<IUserEmailResolver>()
            .Verify(x => x.ResolveEmailsAsync(envelope.UserIdsOrEmails), Times.Once);

        Mocker.GetMock<INotifyService>()
            .Verify(x => x.SendEmail(It.IsAny<EmailNotificationMessage>()), Times.Never);
    }
}