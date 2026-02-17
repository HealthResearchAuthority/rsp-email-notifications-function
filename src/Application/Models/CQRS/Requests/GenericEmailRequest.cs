using Rsp.NotifyFunction.Application.Contracts;

namespace Rsp.NotifyFunction.Application.Models.CQRS.Requests;
public record GenericEmailRequest(
    string EventType,
    string EmailTemplateId,
    IEnumerable<string> UserIds) : IEmailRequest;