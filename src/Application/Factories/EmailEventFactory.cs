using System.Text.Json;
using Rsp.NotifyFunction.Application.Contracts;
using Rsp.NotifyFunction.Application.Models.CQRS;
using Rsp.NotifyFunction.Application.Models.CQRS.Requests;

namespace Rsp.NotifyFunction.Application.Factories;

public class EmailEventFactory : IEmailRequestFactory
{
    /// <summary>
    /// Creates an email request object based on the event type in the envelope.
    /// </summary>
    /// <param name="envelope"></param>
    /// <returns></returns>
    public IEmailRequest? Create(EmailEnvelope envelope)
    {
        // based on the event type in the envelope,
        // create the appropriate email request object and
        // populate it with the data from the envelope
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