using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

using Microsoft.Extensions.DependencyInjection;

using Netlifly.Shared;

using Netlify.ApiClient.Auth;
using Netlify.ApiClient.Hero;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Netlify.ApiClient
{
    public static class ApiClientExtension
    {
        public static void InitApiClient<TConfigImplementation>(this IServiceCollection services, string serverEndPoint)
            where TConfigImplementation : class, IAppConfig
        {
            services.AddSingleton<IAuthRepository, AuthRepository>();

            services.AddSingleton<IAppConfig, TConfigImplementation>();

            //services.AddNewtonsoftJson(
            //    options =>
            //        {
            //            options.SerializerSettings.ContractResolver = new DefaultContractResolver()
            //                                                              {
            //                                                                  NamingStrategy =
            //                                                                      new CamelCaseNamingStrategy()
            //                                                              };
            //            options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
            //            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            //            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //            options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
            //        });
            // Register the GraphQL client with Microsoft serializer
            //services.AddScoped(sp => new GraphQLHttpClient("http://myserver.com/graphql", new SystemTextJsonSerializer()));

            // Register the GraphQL client with Newtonsoft.Json serializer
            services.AddSingleton(sp =>
                {
                    // Resolve the IAppConfig instance
                    var appConfig = sp.GetRequiredService<IAppConfig>();
                    var uriString = $"{serverEndPoint}/{appConfig.Endpoints.Graphql}";

                    var httpClient = new HttpClient { BaseAddress = new Uri(uriString) };
                    var httpClientOptions = new GraphQLHttpClientOptions { EndPoint = new Uri(uriString) };
                    var graphQlHttpClient = new GraphQLHttpClient(
                        httpClientOptions,
                        new NewtonsoftJsonSerializer(),
                        httpClient);

                    // Create and return the GraphQLHttpClient with the appropriate configuration
                    //var graphQlHttpClient = new GraphQLHttpClient($"{serverEndPoint}/{appConfig.Endpoints.Graphql}", new NewtonsoftJsonSerializer());
                    return graphQlHttpClient;
                });

            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IHeroService, HeroService>();
        }
    }
}
