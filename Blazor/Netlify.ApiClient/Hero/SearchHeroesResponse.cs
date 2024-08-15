using GraphQL.Types.Relay.DataObjects;
using Newtonsoft.Json;

namespace Netlify.ApiClient.Hero;

public class SearchHeroesResponse
{
    [JsonProperty(PropertyName = "searchHeroes")]
    public SearchHeroesData SearchHeroes { get; set; }

    public class SearchHeroesData
    {
        public List<HeroEdge> Edges { get; set; }
        public PageInfo PageInfo { get; set; }
        public int TotalCount { get; set; }
    }

    public class HeroEdge
    {
        public string Cursor { get; set; }
        public Netlifly.Shared.Hero Node { get; set; }
    }
}

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class Data
{
    public SearchHeroes searchHeroes { get; set; }
}
public class SearchHeroes
{
    public List<Edge> edges { get; set; }
    public PageInfo pageInfo { get; set; }
    public int totalCount { get; set; }
}
public class Edge
{
    public string cursor { get; set; }
    public Node node { get; set; }
}

public class Node
{
    public string alterEgo { get; set; }
    public string id { get; set; }
    public string image { get; set; }
    public bool @public { get; set; }
    public string realName { get; set; }
    public string userId { get; set; }
    public object votes { get; set; }
    public List<object> usersVoted { get; set; }
}

public class PageInfo
{
    public string endCursor { get; set; }
    public bool hasNextPage { get; set; }
    public bool hasPreviousPage { get; set; }
    public string startCursor { get; set; }
}

public class Root
{
    public Data data { get; set; }
}



