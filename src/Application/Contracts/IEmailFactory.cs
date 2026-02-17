using Rsp.NotifyFunction.Application.Models.CQRS;

namespace Rsp.NotifyFunction.Application.Contracts;

/// <summary>
/// Defines a factory interface for creating email request objects based on the details provided in an email envelope.
/// </summary>
public interface IEmailRequestFactory
{
    IEmailRequest? Create(EmailEnvelope envelope);
}