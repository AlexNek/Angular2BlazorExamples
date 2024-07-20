namespace Netlifly.Shared
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using GraphQL.Client.Http;
    using Newtonsoft.Json;

    using Netlifly.Shared.Response;
    using Netlifly.Shared.Request;
    using System.IdentityModel.Tokens.Jwt;

    public class AuthService
    {
        private readonly GraphQLHttpClient _apollo;
        private readonly AuthRepository _authRepository;

        private readonly IAppConfig _appConfig;

        public AuthService(GraphQLHttpClient apollo, AuthRepository authRepository, IAppConfig appConfig)
        {
            _apollo = apollo;
            _authRepository = authRepository;
            _appConfig = appConfig;
        }

        public static dynamic DecodeToken(string token)
        {
            try
            {
                return JsonConvert.DeserializeObject<dynamic>(JwtDecoder.Decode(token));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IObservable<AuthUserData> Signup(RegisterPayload payload)
        {
            var request = new GraphQLHttpRequest(Mutations.SignupMutation)
                              {
                                  Variables = new { payload.FirstName, payload.Email, payload.Password }
                              };
            

            //var request = new GraphQLHttpRequest
            //{
            //    Query = Mutations.SignupMutation,
            //    Variables = new { payload.FirstName, payload.Email, payload.Password }
            //};

            return _apollo.SendMutationAsync<RegisterResponse>(request)
                .ToObservable()
                .Select(response =>
                {
                    var registerData = response.Data?.Data?.Signup;
                    if (registerData != null)
                    {
                        SaveUserData(registerData);
                        return registerData;
                    }
                    return null;
                });
        }

        public IObservable<AuthUserData> LogIn(string email, string password)
        {
            var request = new GraphQLHttpRequest(Mutations.LoginMutation)
            {
                Variables = new { email, password }
            };

            return _apollo.SendMutationAsync<LogInResponse>(request)
                .ToObservable()
                .Select(response =>
                {
                    var loginData = response.Data?.Data?.Login;
                    if (loginData != null)
                    {
                        SaveUserData(loginData);
                        return loginData;
                    }
                    return null;
                });
        }

        public IObservable<User> UpdateUser(UpdateUserData userData)
        {
            var request = new GraphQLHttpRequest(Mutations.UpdateUserMutation)
            {
                Variables = userData
            };

            return _apollo.SendMutationAsync<UpdateUserResponse>(request)
                .ToObservable()
                .Select(response =>
                {
                    var updateUserData = response.Data?.Data?.UpdateUser;
                    if (updateUserData != null)
                    {
                        _authRepository.SetUser(updateUserData);
                        return updateUserData;
                    }
                    return null;
                });
        }

        public IObservable<OkData> ChangePassword(string oldPassword, string newPassword)
        {
            var request = new GraphQLHttpRequest(Mutations.ChangePasswordMutation)
            {
                Variables = new { oldPassword, newPassword }
            };

            return _apollo.SendMutationAsync<ChangePasswordResponse>(request)
                .ToObservable()
                .Select(response =>
                {
                    var changePasswordData = response.Data?.Data?.ChangePassword;
                    if (changePasswordData != null)
                    {
                        return changePasswordData;
                    }
                    return null;
                });
        }

        public IObservable<OkData> DeleteAccount(string password)
        {
            var request = new GraphQLHttpRequest(Mutations.DeleteAccountMutation)
            {
                Variables = new { password }
            };

            return _apollo.SendMutationAsync<DeleteAccountResponse>(request)
                .ToObservable()
                .Select(response =>
                {
                    var deleteAccountData = response.Data?.Data?.DeleteAccount;
                    if (deleteAccountData != null)
                    {
                        return deleteAccountData;
                    }
                    return null;
                });
        }

        public IObservable<UpdateTokenData> RefreshToken()
        {
            var refreshToken = _authRepository.GetRefreshTokenValue() ?? string.Empty;
            var request = new GraphQLHttpRequest(Mutations.RefreshTokenMutation)
            {
                Variables = new { refreshToken },
                
                //Headers = new { [AppConfig.BypassAuthorization] = "true" }
            };
            
            _apollo.HttpClient.DefaultRequestHeaders.Add(_appConfig.BypassAuthorization,"true");

            return _apollo.SendMutationAsync<RefreshTokenResponse>(request)
                .ToObservable()
                .Select(response =>
                {
                    var refreshTokenData = response.Data?.Data?.RefreshToken;
                    if (refreshTokenData != null)
                    {
                        _authRepository.UpdateTokens(refreshTokenData.AccessToken, refreshTokenData.RefreshToken);
                        return refreshTokenData;
                    }
                    return null;
                });
        }

        private void SaveUserData(AuthUserData userData)
        {
            _authRepository.UpdateTokens(userData.AccessToken, userData.RefreshToken);
            _authRepository.SetUser(userData.User);
        }
    }

    public class JwtDecoder
    {
        public static string? Decode(string tokenStr)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenStr);
            if (token != null)
            {
                var ret = JsonConvert.SerializeObject(token);
                return ret;
            }
            return null;
        }
    }
}
