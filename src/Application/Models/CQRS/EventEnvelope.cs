using System.Text.Json;

namespace Rsp.NotifyFunction.Application.Models.CQRS;

/// <summary>
/// Represents the structure of the message received from
/// the Azure Service Bus queue, containing details about the email to be sent
/// </summary>
public sealed class EmailEnvelope
{
    public string EmailTemplateId { get; set; } = null!;
    public string EventType { get; init; } = default!;
    public JsonElement Data { get; init; }
}