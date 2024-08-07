using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

using GraphQL;
using GraphQL.Client.Http;

using Netlifly.Shared;
using Netlifly.Shared.Request;
using Netlifly.Shared.Response;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Netlify.ApiClient.Auth
{
    internal class AuthService : IAuthService
    {
        private const string Authorization = "Authorization";

        private readonly GraphQLHttpClient _graphQlClient;

        private readonly IAuthRepository _authRepository;

        private readonly IAppConfig _appConfig;

        public AuthService(GraphQLHttpClient graphQlClient, IAuthRepository authRepository, IAppConfig appConfig)
        {
            _graphQlClient = graphQlClient;
            _authRepository = authRepository;
            _appConfig = appConfig;
        }

        public static dynamic DecodeToken(string? token)
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

        public async Task<AuthUserData?> Signup1Async(RegisterPayload payload)
        {
            var request = new GraphQLHttpRequest(Mutations.SignupMutation)
                              {
                                  Variables = new
                                                  {
                                                      firstname = payload.FirstName,
                                                      email = payload.Email,
                                                      password = payload.Password
                                                  }
                              };

            var authUserData = await _graphQlClient.SendMutationAsync<RegisterResponse>(request)
                                   .ToObservable()
                                   .Select(
                                       response =>
                                           {
                                               var registerData = response.Data?.Signup;
                                               if (registerData != null)
                                               {
                                                   SaveUserData(registerData);
                                                   return registerData;
                                               }

                                               return null;
                                           });
            return authUserData;
        }

        public async Task<AuthUserData?> Signup2Async(RegisterPayload payload)
        {

            var request = new GraphQLHttpRequest(Mutations.SignupMutation)
                              {
                                  Variables = new
                                                  {
                                                      firstname = payload.FirstName,
                                                      email = payload.Email,
                                                      password = payload.Password
                                                  }
                              };

            var authUserData = await _graphQlClient.SendMutationAsync<dynamic>(request)
                                   .ToObservable()
                                   .Select(
                                       response =>
                                           {
                                               // Check for errors in the GraphQL response
                                               if (response.Errors != null && response.Errors.Any())
                                               {
                                                   // Handle the errors as needed
                                                   // For example, you could log them or throw an exception
                                                   throw new Exception(
                                                       "GraphQL errors occurred: " + string.Join(
                                                           ", ",
                                                           response.Errors.Select(
                                                               e => e.Message)));
                                               }

                                               // Proceed with normal processing if there are no errors
                                               //RegisterResponse
                                               RegisterResponse registerData =
                                                   JsonConvert.DeserializeObject<RegisterResponse>(
                                                       response.Data.ToString());
                                               //var registerData = response.Data?.Data?.Data?.Signup;
                                               if (registerData != null)
                                               {
                                                   SaveUserData(registerData.Signup);
                                                   return registerData;
                                               }

                                               return null;
                                           })
                ;
            return authUserData?.Signup;
        }

        public async Task<AuthUserData?> SignupAsync(RegisterPayload payload)
        {
            var request = new GraphQLHttpRequest(Mutations.SignupMutation)
            {
                Variables = new
                {
                    firstname = payload.FirstName,
                    email = payload.Email,
                    password = payload.Password
                }
            };
            var response = await _graphQlClient.SendMutationAsync<RegisterResponse>(request);
            IfErrorThrowException(response, "Registration failed:");
            var signupResponse = response.Data.Signup;
            if (signupResponse != null)
            {
                SaveUserData(signupResponse);
            }

            return signupResponse;
        }

        public async Task<AuthUserData?> SignupBadAsync(RegisterPayload payload)
        {
            var request = new GraphQLHttpRequest(Mutations.SignupMutation)
                              {
                                  Variables = new
                                                  {
                                                      firstname = payload.FirstName,
                                                      email = payload.Email,
                                                      password = payload.Password
                                                  }
                              };

            // Assuming the response JSON is stored in a variable called responseJson
            var responseJson = await _graphQlClient.SendMutationAsync<string>(request); // Get raw JSON response
            var jsonResponse = JObject.Parse(responseJson.Data);

            // Check if there are errors
            var errors = jsonResponse["errors"];
            if (errors != null)
            {
                foreach (var error in errors)
                {
                    // Extract standard error information
                    var message = error["message"]?.ToString();
                    Console.WriteLine($"Error: {message}");

                    // Extract "info" field if it exists
                    var info = error["info"] as JArray;
                    if (info != null)
                    {
                        foreach (var infoItem in info)
                        {
                            Console.WriteLine($"Info: {infoItem}");
                        }
                    }
                }

                // Handle the errors as necessary
                throw new Exception("GraphQL errors occurred.");
            }

            // If no errors, proceed with normal processing
            var data = jsonResponse["data"];
            if (data != null)
            {
                var registerData = data["signup"];
                if (registerData != null)
                {
                    // Map to your RegisterResponse object or handle as needed
                    // SaveUserData(registerData);
                    //return registerData;
                    return null;
                }
            }

            return null;

        }

        //public async Task<AuthUserData?> LogIn2Async(string email, string password)
        //{
        //    var request = new GraphQLHttpRequest(Mutations.LoginMutation) { Variables = new { email, password } };

        //    var userData = await _graphQlClient.SendMutationAsync<dynamic>(request)
        //                       .ToObservable()
        //                       .Select(
        //                           response =>
        //                               {
        //                                   IfErrorThrowException(response, "Login failed:");

        //                                   if (response.Data != null)
        //                                   {
        //                                       LogInResponse loginData =
        //                                           JsonConvert.DeserializeObject<LogInResponse>(
        //                                               response.Data.ToString());
        //                                       if (loginData != null)
        //                                       {
        //                                           SaveUserData(loginData.Login);
        //                                           return loginData;
        //                                       }
        //                                   }

        //                                   return null;
        //                               });
        //    return userData?.Login;
        //}
        public async Task<AuthUserData?> LogInAsync(string email, string password)
        {
            var request = new GraphQLHttpRequest(Mutations.LoginMutation) { Variables = new { email, password } };

            var response = await _graphQlClient.SendMutationAsync<LogInResponse>(request);
            IfErrorThrowException(response, "Login failed:");
    
            AuthUserData? loginResponse = response.Data.Login;
            if (loginResponse != null)
            {
                SaveUserData(loginResponse);
            }
            return loginResponse;
        }


        private static void IfErrorThrowException(IGraphQLResponse response, object? errorHeader)
        {
            //Check if there are any errors in the GraphQL response
            if (response.Errors != null && response.Errors.Any())
            {
                // Handle the errors accordingly
                // Here we log the errors or you could throw an exception
                var errorMessages = string.Join(
                    ", ",
                    response.Errors.Select(e => e.Message));
                Console.WriteLine($"GraphQL errors occurred: {errorMessages}");

                // Optionally, you can throw an exception or return a special error object
                throw new GraphQLException($"{errorHeader} {errorMessages}");
            }
        }

        public async Task<User?> UpdateUserAsync(UpdateUserData userData, string? accessToken)
        {
            //{ "operationName":"updateUser",
            //"variables":{
            //  "id":"0ba75529-a1c4-4f69-98d3-3f7d0a11fd26",
            //  "email":"demo@yahoo.com",
            //  "firstname":"demo2",
            //  "language":"en",
            //  "heroes":[],
            //  "__typename":"User"
            // },
            // "query":"mutation updateUser($firstname: String!, $language: String!) {\n
            // updateUser(data: {firstname: $firstname, language: $language}) {\n
            // id\n
            // firstname\n
            // email\n
            // language\n
            // __typename\n  }\n}"}/
            var request = new GraphQLHttpRequest(Mutations.UpdateUserMutation)
                              {
                                  Variables = new
                                                  {
                                                      id = userData.User.Id,
                                                      firstname = userData.FirstName,
                                                      email = userData.User.Email,
                                                      language = userData.User.Language,
                                                      __typename = "User"
                                                  }
                              };
            
            // Add the access token to the default request headers
            _graphQlClient.HttpClient.DefaultRequestHeaders.Add(Authorization, $"Bearer {accessToken}");
            var response = await _graphQlClient.SendMutationAsync<UpdateUserResponse>(request);
            IfErrorThrowException(response, "Update user failed:");
            var updateUserResponse = response.Data.UpdateUser;
            if (updateUserResponse != null)
            {
                _authRepository.SetUser(updateUserResponse);
                return updateUserResponse;
            }

            //var updateUserResponse = await _graphQlClient.SendMutationAsync<dynamic>(request)
            //                             .ToObservable()
            //                             .Select(
            //                                 response =>
            //                                     {
            //                                         IfErrorThrowException(response, "Update user failed:");
            //                                         UpdateUserResponse updateUserResponse =
            //                                             JsonConvert.DeserializeObject<UpdateUserResponse>(
            //                                                 response.Data.ToString());
            //                                         //var updateUserData = response.Data?.Data?.UpdateUser;
            //                                         if (updateUserResponse != null)
            //                                         {
            //                                             _authRepository.SetUser(updateUserResponse.UpdateUser);
            //                                             return updateUserResponse;
            //                                         }

            //                                         return null;
            //                                     });
            return updateUserResponse;
        }

        public async Task<OkData> ChangePassword(string oldPassword, string newPassword)
        {
            var request = new GraphQLHttpRequest(Mutations.ChangePasswordMutation)
                              {
                                  Variables = new { oldPassword, newPassword }
                              };

            return await _graphQlClient.SendMutationAsync<ChangePasswordResponse>(request)
                       .ToObservable()
                       .Select(
                           response =>
                               {
                                   var changePasswordData = response.Data?.Data?.ChangePassword;
                                   if (changePasswordData != null)
                                   {
                                       return changePasswordData;
                                   }

                                   return null;
                               });
        }

        public async Task<OkData> DeleteAccount(string password)
        {
            var request = new GraphQLHttpRequest(Mutations.DeleteAccountMutation) { Variables = new { password } };

            return await _graphQlClient.SendMutationAsync<DeleteAccountResponse>(request)
                       .ToObservable()
                       .Select(
                           response =>
                               {
                                   var deleteAccountData = response.Data?.Data?.DeleteAccount;
                                   if (deleteAccountData != null)
                                   {
                                       return deleteAccountData;
                                   }

                                   return null;
                               });
        }

        public async Task<UpdateTokenData?> RefreshTokenAsync(string refreshToken1)
        {
            var refreshToken = _authRepository.GetRefreshToken() ?? string.Empty;
            var request = new GraphQLHttpRequest(Mutations.RefreshTokenMutation)
                              {
                                  Variables = new { refreshToken },

                                  //Headers = new { [AppConfig.BypassAuthorization] = "true" }
                              };

            _graphQlClient.HttpClient.DefaultRequestHeaders.Add(_appConfig.BypassAuthorization, "true");

            var response = await _graphQlClient.SendMutationAsync<RefreshTokenResponse>(request);
            IfErrorThrowException(response, "Refresh token failed:");
            var updateUserResponse = response.Data.RefreshToken;
            if (updateUserResponse != null)
            {
                _authRepository.UpdateTokens(
                    updateUserResponse.AccessToken,
                    updateUserResponse.RefreshToken);
            }
            return updateUserResponse;

            //return await _graphQlClient.SendMutationAsync<RefreshTokenResponse>(request)
            //           .ToObservable()
            //           .Select(
            //               response =>
            //                   {
            //                       var refreshTokenData = response.Data?.Data?.RefreshToken;
            //                       if (refreshTokenData != null)
            //                       {
            //                           _authRepository.UpdateTokens(
            //                               refreshTokenData.AccessToken,
            //                               refreshTokenData.RefreshToken);
            //                           return refreshTokenData;
            //                       }

            //                       return null;
            //                   });
        }

        //public async Task<string?> RefreshTokenAsync(string refreshToken)
        //{
        //    return await Task.FromResult(null as string);
        //}

        private void SaveUserData(AuthUserData userData)
        {
            _authRepository.UpdateTokens(userData.AccessToken, userData.RefreshToken);
            _authRepository.SetUser(userData.User);
        }
    }
}
