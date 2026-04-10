namespace Rsp.NotifyFunction.Application.ServiceClients;

public interface IEmailNotificationServiceClient
{
    /// <summary>
    ///     Gets an email notification by Id
    /// </summary>
    [Get("/emailnotifications/{id}")]
    Task<IApiResponse<EmailNotificationDto>> GetEmailNotification(
        Guid id);

    /// <summary>
    ///     Updates an existing email notification
    /// </summary>
    [Put("/emailnotifications/update")]
    Task<IApiResponse<object>> UpdateEmailNotification(
        [Body] EmailNotificationDto request);
}