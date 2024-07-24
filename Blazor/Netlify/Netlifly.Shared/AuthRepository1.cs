namespace Netlifly.Shared
{
    // Translated from TypeScript to C#

    // Framework: Angular
    //using NgNeat.Elf;
    //using NgNeat.ElfPersistState;

    //using Rx;

    //public class AuthRepository1
    //{
    //    private readonly Store _authStore;

    //    private IObservable<User> _user;

    //    public string Locale { get; }

    //    public AuthRepository1(string locale)
    //    {
    //        Locale = locale;
    //        _authStore = new Store(
    //            new { name = "auth" },
    //            Elf.WithProps<IAuthProps>(new AuthProps { User = null, AccessToken = null, RefreshToken = null })
    //        );

    //        PersistState(_authStore, new PersistStateOptions { Key = "auth", Storage = localStorageStrategy });

    //        _user = _authStore.Select(state => state.User);
    //    }

    //    public void Clear()
    //    {
    //        _authStore.Update(Elf.SetProps(new { User = null, AccessToken = null, RefreshToken = null }));
    //    }

    //    public string GetAccessTokenValue()
    //    {
    //        return _authStore.GetValue().AccessToken;
    //    }

    //    public string GetRefreshTokenValue()
    //    {
    //        return _authStore.GetValue().RefreshToken;
    //    }

    //    public IObservable<bool> IsLoggedIn()
    //    {
    //        return _authStore.Select(state => state.AccessToken != null);
    //    }

    //    public bool IsLoggedInValue()
    //    {
    //        try
    //        {
    //            var token = GetAccessTokenValue();
    //            if (token != null)
    //            {
    //                return AuthService.DecodeToken(token) != null;
    //            }

    //            return false;
    //        }
    //        catch (Exception)
    //        {
    //            return false;
    //        }
    //    }

    //    public void SetUser(User user)
    //    {
    //        _authStore.Update(state => new { state.User = user });
    //    }

    //    public void UpdateTokens(string accessToken, string refreshToken)
    //    {
    //        _authStore.Update(Elf.SetProps(new { AccessToken = accessToken, RefreshToken = refreshToken }));
    //    }
    //}
}
