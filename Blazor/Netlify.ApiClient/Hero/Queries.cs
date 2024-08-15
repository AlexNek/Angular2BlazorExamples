namespace Netlify.ApiClient.Hero;

internal static class Queries
{
    public const string SearchHeroesQuery = @"
  query searchHeroes($query: String!, $after: String!, $first: Int!, $direction: OrderDirection!, $field: HeroOrderField!, $skip: Int!) {
                    searchHeroes(query: $query, after: $after, first: $first, orderBy: { direction: $direction, field: $field }, skip: $skip) {
                        edges {
                            cursor
                            node {
                                id
                                realName
                                alterEgo
                                image
                                public
                                usersVoted {
                                    firstname
                                }
                            }
                        }
                        pageInfo {
                            endCursor
                            hasNextPage
                            hasPreviousPage
                            startCursor
                        }
                        totalCount
                    }
                }
";
}