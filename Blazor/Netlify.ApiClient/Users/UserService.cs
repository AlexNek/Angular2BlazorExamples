using GraphQL;
using GraphQL.Client.Http;

using Netlifly.Shared;
using System.Net.Http.Headers;

namespace Netlify.ApiClient.Users
{
    // Adjust the namespace according to your project structure

    internal class UserService : IUserService
    {
        private readonly GraphQLHttpClient _client;

        public UserService(GraphQLHttpClient client)
        {
            _client = client;
        }

        public async Task<User?> GetMeAsync(string accessToken)
        {
            var request = new GraphQLRequest { Query = UserQueries.GetMe };
            _client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _client.SendQueryAsync<GetMeResponse>(request);
            ApiHelper.IfErrorThrowException(response, "Get Me failed:");
            return response.Data?.Me;
        }
    }
}
