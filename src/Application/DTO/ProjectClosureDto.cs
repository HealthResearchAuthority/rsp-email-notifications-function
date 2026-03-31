namespace Rsp.NotifyFunction.Application.DTO;

/// <summary>
///     Represents a request to send a project closure email notification,
///     including key project identifiers for display in the template.
/// </summary>
/// <param name="ShortProjectTitle">The short title of the project</param>
/// <param name="IrasId">The Iras ID identifier of the project</param>
public record ProjectClosureDto(string ShortProjectTitle, string IrasId);