using System.Reactive.Linq;
using System.Reactive.Subjects;

using Microsoft.Extensions.Localization;

using Netlifly.Shared;

using Newtonsoft.Json;
using static GraphQL.Validation.BasicVisitor;

namespace Netlify.ApiClient.Auth
{
    internal class AuthRepository:IAuthRepository
    {
        private readonly BehaviorSubject<AuthProps> authStore;

        //private readonly IStringLocalizer localizer;

        public IObservable<User> UserObservable { get; private set; }

        public AuthRepository(IStringLocalizerFactory localizerFactory)
        {
            //TODO: use shared localizer if needed
            //localizer = localizerFactory.Create(nameof(SharedLocalizer), typeof(AuthRepository).Assembly.FullName);
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

        public User? GetUser()
        {
            var state = authStore.Value;
            return state.User;
        }

        public string? GetAccessToken()
        {
            return authStore.Value.AccessToken;
        }

        public string? GetRefreshToken()
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
                var token = GetAccessToken();
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

        public event Action<User>? UserChanged;

        public void SetUser(User user)
        {
            var state = authStore.Value;
            state.User = user;
            authStore.OnNext(state);
            SaveState();
            OnUserChanged(user); // Notify subscribers
        }

        protected virtual void OnUserChanged(User user)
        {
            UserChanged?.Invoke(user); // Raise the event to notify subscribers
        }

        public void UpdateTokens(string accessToken, string refreshToken)
        {
            var state = authStore.Value;
            state.AccessToken = accessToken;
            state.RefreshToken = refreshToken;
            authStore.OnNext(state);
            SaveState();
        }

        /// <summary>
        /// Loads the state.
        /// The same as in Angular. Not good solution
        /// </summary>
        private void LoadState()
        {
            if (File.Exists("auth.json"))
            {
                var json = File.ReadAllText("auth.json");
                var state = JsonConvert.DeserializeObject<AuthProps>(json);
                authStore.OnNext(state);
            }
        }

        /// <summary>
        /// Saves the state.
        /// The same as in Angular. Not good solution
        /// </summary>
        private void SaveState()
        {
            var json = JsonConvert.SerializeObject(authStore.Value);
            File.WriteAllText("auth.json", json);
        }
    }
}
