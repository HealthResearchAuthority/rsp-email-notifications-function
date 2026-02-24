using Rsp.NotifyFunction.Application.Contracts;
using Rsp.NotifyFunction.Application.DTO;

namespace Rsp.NotifyFunction.Application.EventRouters;

public class EmailHandlerRouter : IEmailHandlerRouter
{
    private readonly Dictionary<string, IEmailHandler> _handlers;

    public EmailHandlerRouter(IEnumerable<IEmailHandler> handlers)
    {
        _handlers = handlers.ToDictionary(h => h.EventType);
    }

    public Task Route(EmailEnvelope envelope)
    {
        if (!_handlers.TryGetValue(envelope.EventType, out var handler))
            return Task.CompletedTask; // ignore unknown events

        return handler.Handle(envelope);
    }
}