namespace Rsp.NotifyFunction.UnitTests.Application.EmailHandlers;

public class ProjectClosureHandlerTests : TestServiceBase<ProjectClosureHandler>
{
    [Fact]
    public void EventType_ReturnsProjectClosure()
    {
        // Assert
        Sut.EventType.ShouldBe(NotificationTypes.ProjectClosure);
    }

    [Fact]
    public async Task Handle_WhenUserIdsOrEmailsIsEmpty_DoesNotResolveEmails_AndDoesNotSendEmails()
    {
        // Arrange
        var envelope = new EmailEnvelope
        {
            EventType = NotificationTypes.ProjectClosure,
            EmailTemplateId = "template-123",
            UserIdsOrEmails = [],
            Data = JsonSerializer.SerializeToElement(new ProjectClosureDto("IRAS-001", "Project A"))
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
    public async Task Handle_WhenResolvedEmailsExist_SendsOneEmailPerRecipient_WithProjectClosureData()
    {
        // Arrange
        var envelope = new EmailEnvelope
        {
            EventType = NotificationTypes.ProjectClosure,
            EmailTemplateId = "template-123",
            UserIdsOrEmails = ["user-1", "user-2"],
            Data = JsonSerializer.SerializeToElement(new ProjectClosureDto("IRAS-123", "Cancer Study"))
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
            .Verify(x => x.SendEmail(It.IsAny<EmailNotificationMessage>()), Times.Exactly(2));

        var sentMessages = Mocker.GetMock<INotifyService>()
            .Invocations
            .Where(i => i.Method.Name == nameof(INotifyService.SendEmail))
            .Select(i => i.Arguments[0].ShouldBeOfType<EmailNotificationMessage>())
            .ToList();

        sentMessages.ShouldContain(m =>
            m.RecipientAddress == "one@test.com" &&
            m.EmailTemplateId == envelope.EmailTemplateId &&
            m.EventType == envelope.EventType &&
            m.Data.ContainsKey("iras_id") &&
            m.Data.ContainsKey("short_title"));

        sentMessages.ShouldContain(m =>
            m.RecipientAddress == "two@test.com" &&
            m.EmailTemplateId == envelope.EmailTemplateId &&
            m.EventType == envelope.EventType &&
            m.Data.ContainsKey("iras_id") &&
            m.Data.ContainsKey("short_title"));
    }

    [Fact]
    public async Task Handle_WhenResolvedEmailsIsEmpty_DoesNotSendEmails()
    {
        // Arrange
        var envelope = new EmailEnvelope
        {
            EventType = NotificationTypes.ProjectClosure,
            EmailTemplateId = "template-123",
            UserIdsOrEmails = ["user-1"],
            Data = JsonSerializer.SerializeToElement(new ProjectClosureDto("IRAS-001", "Project A"))
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

    [Fact]
    public async Task Handle_WhenResolvedEmailsExist_CallsSendEmailForEachResolvedEmail()
    {
        // Arrange
        var envelope = new EmailEnvelope
        {
            EventType = NotificationTypes.ProjectClosure,
            EmailTemplateId = "template-123",
            UserIdsOrEmails = ["user-1", "user-2", "user-3"],
            Data = JsonSerializer.SerializeToElement(new ProjectClosureDto("IRAS-001", "Project A"))
        };

        Mocker.GetMock<IUserEmailResolver>()
            .Setup(x => x.ResolveEmailsAsync(envelope.UserIdsOrEmails))
            .ReturnsAsync(["one@test.com", "two@test.com", "three@test.com"]);

        // Act
        await Sut.Handle(envelope);

        // Assert
        Mocker.GetMock<INotifyService>()
            .Verify(x => x.SendEmail(It.IsAny<EmailNotificationMessage>()), Times.Exactly(3));
    }
}