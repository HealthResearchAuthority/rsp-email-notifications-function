using MediatR;

namespace Rsp.NotifyFunction.Application.Contracts;

public interface IEmailRequest : IRequest
{
    string EventType { get; }
    string EmailTemplateId { get; }
    IEnumerable<string> UserIds { get; }
}