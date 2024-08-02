using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

using Microsoft.Extensions.DependencyInjection;

using Netlifly.Shared;

using Netlify.ApiClient.Auth;

namespace Netlify.ApiClient
{
    public static class ApiClientExtension
    {
        public static void InitApiClient<TConfigImplementation>(this IServiceCollection services, string serverEndPoint)
            where TConfigImplementation : class, IAppConfig
        {
            services.AddScoped<AuthRepository>();

            services.AddScoped<IAppConfig, TConfigImplementation>();

            // Register the GraphQL client with Microsoft serializer
            //services.AddScoped(sp => new GraphQLHttpClient("http://myserver.com/graphql", new SystemTextJsonSerializer()));

            // Register the GraphQL client with Newtonsoft.Json serializer
            services.AddScoped(sp =>
                {
                    // Resolve the IAppConfig instance
                    var appConfig = sp.GetRequiredService<IAppConfig>();
                    var httpClient = new HttpClient
                    {
                        BaseAddress = new Uri($"{serverEndPoint}/{appConfig.Endpoints.Graphql}")
                    };
                    var httpClientOptions = new GraphQLHttpClientOptions { EndPoint = new Uri($"{serverEndPoint}/{appConfig.Endpoints.Graphql}") };
                    var graphQlHttpClient = new GraphQLHttpClient(
                        httpClientOptions,
                        new NewtonsoftJsonSerializer(),
                        httpClient);

                    // Create and return the GraphQLHttpClient with the appropriate configuration
                    //var graphQlHttpClient = new GraphQLHttpClient($"{serverEndPoint}/{appConfig.Endpoints.Graphql}", new NewtonsoftJsonSerializer());
                    return graphQlHttpClient;
                });

            services.AddScoped<IAuthService, AuthService>();
        }
    }
}
