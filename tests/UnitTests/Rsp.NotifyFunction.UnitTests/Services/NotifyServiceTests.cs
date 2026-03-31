namespace Rsp.NotifyFunction.UnitTests.Services;

public class NotifyServiceTests : TestServiceBase<NotifyService>
{
    [Fact]
    public async Task GetNotificationStatus_ShouldReturnNotificationFromClient()
    {
        // Arrange
        var notificationId = "notification-123";

        var expectedNotification = new Notification
        {
            id = notificationId
        };

        Mocker.GetMock<IAsyncNotificationClient>()
            .Setup(x => x.GetNotificationByIdAsync(notificationId))
            .ReturnsAsync(expectedNotification);

        // Act
        var result = await Sut.GetNotificationStatus(notificationId);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(expectedNotification);

        Mocker.GetMock<IAsyncNotificationClient>()
            .Verify(x => x.GetNotificationByIdAsync(notificationId), Times.Once);
    }

    [Fact]
    public async Task SendEmail_ShouldCallNotifyClientWithMappedValues_AndReturnResponse()
    {
        // Arrange
        var emailNotificationMessage = new EmailNotificationMessage
        {
            RecipientAddress = "test@example.com",
            EmailTemplateId = "template-123",
            Data = new Dictionary<string, object>
            {
                { "name", "Research" },
                { "project", "Test Project" }
            }
        };

        var expectedResponse = new EmailNotificationResponse
        {
            id = "email-123"
        };

        Dictionary<string, dynamic>? capturedData = null;
        string? capturedRecipient = null;
        string? capturedTemplateId = null;

        Mocker.GetMock<IAsyncNotificationClient>()
            .Setup(x => x.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, dynamic>>(),
                null,
                null,
                null))
            .Callback<string, string, Dictionary<string, dynamic>, string?, string?, string?>((recipient, templateId,
                data, _, _, _) =>
            {
                capturedRecipient = recipient;
                capturedTemplateId = templateId;
                capturedData = data;
            })
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await Sut.SendEmail(emailNotificationMessage);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(expectedResponse);

        capturedRecipient.ShouldBe("test@example.com");
        capturedTemplateId.ShouldBe("template-123");
        capturedData.ShouldNotBeNull();
        capturedData.Count.ShouldBe(2);

        Mocker.GetMock<IAsyncNotificationClient>()
            .Verify(x => x.SendEmailAsync(
                    emailNotificationMessage.RecipientAddress,
                    emailNotificationMessage.EmailTemplateId,
                    It.IsAny<Dictionary<string, dynamic>>(),
                    null,
                    null,
                    null),
                Times.Once);
    }

    [Fact]
    public async Task SendEmail_WhenDataIsEmpty_ShouldSendEmptyDictionary()
    {
        // Arrange
        var emailNotificationMessage = new EmailNotificationMessage
        {
            RecipientAddress = "test@example.com",
            EmailTemplateId = "template-123",
            Data = new Dictionary<string, object>()
        };

        var expectedResponse = new EmailNotificationResponse
        {
            id = "email-456"
        };

        Dictionary<string, dynamic>? capturedData = null;

        Mocker.GetMock<IAsyncNotificationClient>()
            .Setup(x => x.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, dynamic>>(),
                null,
                null,
                null))
            .Callback<string, string, Dictionary<string, dynamic>, string?, string?, string?>((_, _, data, _, _, _) =>
            {
                capturedData = data;
            })
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await Sut.SendEmail(emailNotificationMessage);

        // Assert
        result.ShouldBe(expectedResponse);
        capturedData.ShouldNotBeNull();
        capturedData.ShouldBeEmpty();

        Mocker.GetMock<IAsyncNotificationClient>()
            .Verify(x => x.SendEmailAsync(
                    emailNotificationMessage.RecipientAddress,
                    emailNotificationMessage.EmailTemplateId,
                    It.IsAny<Dictionary<string, dynamic>>(),
                    null,
                    null,
                    null),
                Times.Once);
    }
}