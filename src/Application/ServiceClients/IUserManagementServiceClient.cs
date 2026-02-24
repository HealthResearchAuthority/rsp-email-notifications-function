using Refit;
using Rsp.NotifyFunction.Application.DTO;

namespace Rsp.NotifyFunction.Application.ServiceClients;

public interface IUserManagementServiceClient
{
    /// <summary>
    /// Gets users by their ids database
    /// </summary>
    /// <returns>List of users</returns>
    [Post("/users/by-ids")]
    public Task<IApiResponse<UsersResponse>> GetUsersById
    (
        [Body] IEnumerable<string> ids,
        string? searchQuery = null,
        int pageIndex = 1,
        int pageSize = 10
    );
}