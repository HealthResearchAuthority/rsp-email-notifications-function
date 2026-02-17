using System.Text.Json;
using Rsp.NotifyFunction.Application.Contracts;
using Rsp.NotifyFunction.Application.Models.CQRS;
using Rsp.NotifyFunction.Application.Models.CQRS.Requests;

namespace Rsp.NotifyFunction.Application.Factories;

public class EmailEventFactory : IEmailRequestFactory
{
    public IEmailRequest? Create(EmailEnvelope envelope)
    {
        return envelope.EventType switch
        {
            "GenericNotification" =>
            envelope.Data.Deserialize<GenericEmailRequest>() with
            {
                EventType = envelope.EventType,
                EmailTemplateId = envelope.EmailTemplateId
            },

            _ => null
        };
    }
}