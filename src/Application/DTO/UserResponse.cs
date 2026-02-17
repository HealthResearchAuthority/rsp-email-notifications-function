using Rsp.NotifyFunction.Domain;

namespace Rsp.NotifyFunction.Application.DTO;

public class UsersResponse
{
    public IEnumerable<User> Users { get; set; } = [];

    public IEnumerable<string> UserIds { get; set; } = [];

    public int TotalCount { get; set; }
}