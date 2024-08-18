using Netlifly.Shared;

namespace Netlify.ApiClient.Users;

public interface IUserService
{
    Task<User?> GetMeAsync(string accessToken);
}
