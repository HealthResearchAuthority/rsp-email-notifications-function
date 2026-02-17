using System.Text.Json;

namespace Rsp.NotifyFunction.Application.Models.CQRS;

public sealed class EmailEnvelope
{
    public string EmailTemplateId { get; set; } = null!;
    public string EventType { get; init; } = default!;
    public JsonElement Data { get; init; }
}