using MediatR;

namespace Rsp.NotifyFunction.Application.Contracts;

/// <summary>
/// Defines the contract for an email request which can be used as
/// the base for different types of email requests in the application.
/// </summary>
public interface IEmailRequest : IRequest
{
    string EventType { get; }
    string EmailTemplateId { get; }
    IEnumerable<string> UserIds { get; }
}