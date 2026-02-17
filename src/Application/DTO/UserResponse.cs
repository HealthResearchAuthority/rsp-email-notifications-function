using Rsp.NotifyFunction.Domain;

namespace Rsp.NotifyFunction.Application.DTO;

/// <summary>
/// Represents the response from the user management service when retrieving user details by their IDs,
/// </summary>
public class UsersResponse
{
    public IEnumerable<User> Users { get; set; } = [];

    public IEnumerable<string> UserIds { get; set; } = [];

    public int TotalCount { get; set; }
}