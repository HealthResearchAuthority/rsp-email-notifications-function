namespace Rsp.NotifyFunction.Application.Contracts;

public interface IUserEmailResolver
{
    Task<List<string>> ResolveEmailsAsync(IEnumerable<string>? userIdsOrEmails);
}