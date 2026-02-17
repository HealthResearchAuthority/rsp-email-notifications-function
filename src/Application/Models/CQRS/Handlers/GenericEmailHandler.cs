using MediatR;
using Rsp.NotifyFunction.Application.Contracts;
using Rsp.NotifyFunction.Application.DTO;
using Rsp.NotifyFunction.Application.Models.CQRS.Requests;
using Rsp.NotifyFunction.Application.ServiceClients;

namespace Rsp.NotifyFunction.Application.Models.CQRS.Handlers;

public class GenericEmailHandler(
    INotifyService notifyService,
    IUserManagementServiceClient userManagementClient)
    : IRequestHandler<GenericEmailRequest>
{
    /// <summary>
    /// Handles the generic email request by retrieving necessary data and sending an email notification.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task Handle(GenericEmailRequest request, CancellationToken cancellationToken)
    {
        var userIds = request.UserIds;

        // Retrieve user details for the provided user IDs using the user management service client
        var usersResponse = await userManagementClient.GetUsersById(userIds, pageIndex: 1, pageSize: 1000);
        if (usersResponse.IsSuccessStatusCode
            && usersResponse.Content != null)
        {
            // For each user retrieved,
            // create an email notification message and send it using the notify service
            // Notify service does not provide programatic bulk sending capability,
            // so we need to send individual messages for each recipient
            foreach (var user in usersResponse.Content.Users)
            {
                var message = new EmailNotificationMessage
                {
                    EmailTemplateId = request.EmailTemplateId,
                    EventType = request.EventType.ToString(),
                    RecipientAdress = user.Email,
                    Data = new Dictionary<string, dynamic>
                     {
                         { "firstName", user.GivenName },
                         { "lastName", user.FamilyName }
                     }
                };

                // Send the email notification message using the notify service
                var sendEmail = await notifyService.SendEmail(message);
            }
        }
    }
}