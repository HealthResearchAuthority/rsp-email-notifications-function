namespace Rsp.NotifyFunction.Services;

public class UserEmailResolver(IUserManagementServiceClient userManagementServiceClient) : IUserEmailResolver
{
    public async Task<List<string>> ResolveEmailsAsync(IEnumerable<string>? userIdsOrEmails)
    {
        var emails = new List<string>();
        var ids = new List<string>();

        foreach (var value in userIdsOrEmails ?? [])
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            if (Guid.TryParse(value, out var guid))
            {
                ids.Add(guid.ToString());
            }
            else
            {
                emails.Add(value);
            }
        }

        if (ids.Any())
        {
            var usersResponse = await userManagementServiceClient.GetUsersById(
                ids,
                pageIndex: 1,
                pageSize: 1000);

            if (usersResponse is { IsSuccessStatusCode: true, Content: not null })
            {
                emails.AddRange(usersResponse.Content.Users
                    .Where(u => !string.IsNullOrWhiteSpace(u.Email))
                    .Select(u => u.Email));
            }
        }

        return emails
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .Select(e => e.Trim().ToLowerInvariant())
            .Distinct()
            .ToList();
    }
}