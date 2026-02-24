namespace Rsp.NotifyFunction.Application.DTO;

/// <summary>
/// Represents a request to send a generic email notification
/// with a link to the user notification dashboard
/// </summary>
/// <param name="DashboardUrl"></param>
public record GenericEmailData(string DashboardUrl);