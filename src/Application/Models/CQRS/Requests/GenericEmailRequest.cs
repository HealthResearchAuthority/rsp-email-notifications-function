using Rsp.NotifyFunction.Application.Contracts;

namespace Rsp.NotifyFunction.Application.Models.CQRS.Requests;

/// <summary>
/// Represents a request to send a generic email notification
/// to a list of users based on a specified email template and event type.
/// </summary>
/// <param name="EventType"></param>
/// <param name="EmailTemplateId"></param>
/// <param name="UserIds"></param>
public record GenericEmailRequest(
    string EventType,
    string EmailTemplateId,
    IEnumerable<string> UserIds) : IEmailRequest;