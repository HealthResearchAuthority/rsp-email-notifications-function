namespace Rsp.NotifyFunction.Application.DTO;

public class EmailNotificationDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = EmailNotificationStatuses.Queued;
    public string? ErrorMessage { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? FailedAt { get; set; }
}