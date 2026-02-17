using Rsp.NotifyFunction.Application.Models.CQRS;

namespace Rsp.NotifyFunction.Application.Contracts;

public interface IEmailRequestFactory
{
    IEmailRequest? Create(EmailEnvelope envelope);
}