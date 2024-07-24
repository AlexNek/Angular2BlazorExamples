using System.Reactive.Linq;
using System.Reactive.Subjects;

using Microsoft.Extensions.Localization;

using Newtonsoft.Json;

namespace Netlifly.Shared
{
    public class AuthRepository
    {
        private readonly BehaviorSubject<AuthProps> authStore;

        private readonly IStringLocalizer localizer;

        public IObservable<User> UserObservable { get; private set; }

        public AuthRepository(IStringLocalizerFactory localizerFactory)
        {
            localizer = localizerFactory.Create("AuthRepository", typeof(AuthRepository).Assembly.FullName);
            authStore = new BehaviorSubject<AuthProps>(
                new AuthProps { User = null, AccessToken = null, RefreshToken = null });

            LoadState();

            UserObservable = authStore.Select(state => state.User);
        }

        public void Clear()
        {
            var state = authStore.Value;
            state.User = null;
            state.AccessToken = null;
            state.RefreshToken = null;
            authStore.OnNext(state);
            SaveState();
        }

        public string GetAccessTokenValue()
        {
            return authStore.Value.AccessToken;
        }

        public string GetRefreshTokenValue()
        {
            return authStore.Value.RefreshToken;
        }

        public IObservable<bool> IsLoggedIn()
        {
            return authStore.Select(state => !string.IsNullOrEmpty(state.AccessToken));
        }

        public bool IsLoggedInValue()
        {
            try
            {
                var token = GetAccessTokenValue();
                if (!string.IsNullOrEmpty(token))
                {
                    return AuthService.DecodeToken(token) != null;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SetUser(User user)
        {
            var state = authStore.Value;
            state.User = user;
            authStore.OnNext(state);
            SaveState();
        }

        public void UpdateTokens(string accessToken, string refreshToken)
        {
            var state = authStore.Value;
            state.AccessToken = accessToken;
            state.RefreshToken = refreshToken;
            authStore.OnNext(state);
            SaveState();
        }

        private void LoadState()
        {
            var json = File.ReadAllText("auth.json");
            var state = JsonConvert.DeserializeObject<AuthProps>(json);
            authStore.OnNext(state);
        }

        private void SaveState()
        {
            var json = JsonConvert.SerializeObject(authStore.Value);
            File.WriteAllText("auth.json", json);
        }
    }
}
