namespace Rsp.NotifyFunction.Application.DTO;

/// <summary>
///     Represents a request to send a revision requested email notification,
///     including key project identifiers for display in the template.
/// </summary>
/// <param name="ShortProjectTitle">The short title of the project</param>
/// <param name="ModificationId">The Modification ID identifier of the project</param>
public record RevisionRequestedDto(string ShortProjectTitle, string ModificationId);