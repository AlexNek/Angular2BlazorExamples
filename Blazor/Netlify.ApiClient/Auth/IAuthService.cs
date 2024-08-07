using Netlifly.Shared;
using Netlifly.Shared.Request;
using Netlifly.Shared.Response;

namespace Netlify.ApiClient.Auth;

public interface IAuthService
{
    Task<AuthUserData?> SignupAsync(RegisterPayload payload);

    Task<AuthUserData?> LogInAsync(string email, string password);

    Task<User?> UpdateUserAsync(UpdateUserData userData, string? accessToken);

    Task<OkData> ChangePassword(string oldPassword, string newPassword);

    Task<OkData> DeleteAccount(string password);

    Task<UpdateTokenData?> RefreshTokenAsync(string refreshToken);

    //Task<string?> RefreshTokenAsync(string refreshToken);
}
