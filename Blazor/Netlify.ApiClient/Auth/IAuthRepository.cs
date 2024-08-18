using Netlifly.Shared;

namespace Netlify.ApiClient.Auth;

public interface IAuthRepository
{
    event Action<User> UserChanged;
    void SetUser(User user);
    Task<User?> GetUserAsync();
    string? GetAccessToken();
    string? GetRefreshToken();
    void UpdateTokens(string accessToken, string refreshToken);
    IObservable<bool> IsLoggedIn();
    void Clear();
}
