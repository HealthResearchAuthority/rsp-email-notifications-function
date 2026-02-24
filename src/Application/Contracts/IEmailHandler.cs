using Rsp.NotifyFunction.Application.DTO;

namespace Rsp.NotifyFunction.Application.Contracts;

public interface IEmailHandler
{
    string EventType { get; }

    Task Handle(EmailEnvelope envelope);
}