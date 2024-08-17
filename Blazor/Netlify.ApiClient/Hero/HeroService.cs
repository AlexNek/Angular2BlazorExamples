using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using System.Net.Http.Headers;
using static Netlify.ApiClient.Hero.SearchHeroesRequest;

namespace Netlify.ApiClient.Hero;

internal class HeroService : IHeroService
{
    private readonly GraphQLHttpClient _client;

    public HeroService(GraphQLHttpClient client)
    {
        _client = client;
    }

    public async Task<SearchHeroesResponse.SearchHeroesData?> SearchHeroesAsync(SearchHeroesRequest options)
    {
        Dictionary<OrderDirection, string> orderDirectionValues = new Dictionary<OrderDirection, string>
                                                                      {
                                                                          { OrderDirection.Asc, "asc" },
                                                                          { OrderDirection.Desc, "desc" }
                                                                      };

        Dictionary<HeroOrderField, string> heroOrderFieldValues = new Dictionary<HeroOrderField, string>
                                                                      {
                                                                          { HeroOrderField.UsersVoted, "usersVoted" },
                                                                          
                                                                      };
        var request = new GraphQLRequest
                          {
                              Query = Queries.SearchHeroesQuery,
                              Variables = new
                                              {
                                                  query = options.Query,
                                                  after = options.After,
                                                  first = options.First,
                                                  direction = orderDirectionValues[options.OrderBy.Direction],
                                                  field = heroOrderFieldValues[options.OrderBy.Field],
                                                  skip = options.Skip
                                              }
                          };

        var response = await _client.SendQueryAsync<SearchHeroesResponse>(request);
        IfErrorThrowException(response, "SearchHeroes failed:");
        return response.Data.SearchHeroes;

        //return _client.SendQueryAsync<SearchHeroesResponse>(request)
        //    .Select(response =>
        //        {
        //            var searchHeroesData = response.Data?.SearchHeroes;
        //            if (searchHeroesData != null && searchHeroesData.Edges?.Any() == true)
        //            {
        //                return searchHeroesData.Edges.Select(edge => edge.Node).ToList();
        //            }
        //            return new List<Hero>();
        //        });
    }

    public async Task<Netlifly.Shared.Hero?> CreateHero(CreateHeroData data)
    {
        var request = new GraphQLRequest
                          {
                              Query = Mutations.CreateHeroMutation,
                              Variables = data
                          };
        var response = await _client.SendMutationAsync<CreateHeroResponse>(request);
        IfErrorThrowException(response, "CreateHero failed:");
        return response.Data?.CreateHero;
        //return await _client.SendMutationAsync<CreateHeroResponse>(request)
        //    .Select(response => response.Data?.CreateHero);
    }

    public async Task<DeleteHeroResponse.DeleteHeroData?> DeleteHero(string heroId)
    {
        var request = new GraphQLRequest
                          {
                              Query = Mutations.DeleteHeroMutation,
                              Variables = new { heroId }
                          };
        var response = await _client.SendMutationAsync<DeleteHeroResponse>(request);
        IfErrorThrowException(response, "DeleteHero failed:");
        return response.Data?.RemoveHero;
        //return await _client.SendMutationAsync<DeleteHeroResponse>(request)
        //    .Select(response => response.Data?.RemoveHero);
    }

    public async Task<Netlifly.Shared.Hero?> VoteForHeroAsync(string heroId, string? accessToken)
    {
        var request = new GraphQLRequest
                          {
                              Query = Mutations.VoteForHeroMutation,
                              Variables = new { heroId }
                          };

        _client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await _client.SendMutationAsync<VoteForHeroResponse>(request);
        IfErrorThrowException(response, "VoteForHero failed:");
        return response.Data?.VoteHero;
        //return await _client.SendMutationAsync<VoteForHeroResponse>(request)
        //    .Select(response => response.Data?.VoteHero);
    }
    public async Task<int?> GetVoteForHeroAsync(string heroId)
    {
        var request = new GraphQLRequest
                          {
                              Query = Queries.GetVoteForHero,
                              Variables = new { heroId }
                          };

        
        var response = await _client.SendQueryAsync<GetVoteForHeroResponse>(request);
        IfErrorThrowException(response, "GetVoteForHero failed:");
        return response.Data?.HeroVotes.Votes;
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
}