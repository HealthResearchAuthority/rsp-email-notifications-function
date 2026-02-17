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
    public async Task Handle(GenericEmailRequest request, CancellationToken cancellationToken)
    {
        // here get the data we need for the email from the various services
        var userIds = request.UserIds;

        var usersResponse = await userManagementClient.GetUsersById(userIds, pageIndex: 1, pageSize: 1000);
        if (usersResponse.IsSuccessStatusCode
            && usersResponse.Content != null)
        {
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

                var sendEmail = await notifyService.SendEmail(message);
            }
        }
    }
}