namespace Rsp.NotifyFunction.Application.Contracts;

public interface IEmailHandlerRouter
{
    Task Route(EmailEnvelope envelope);
}