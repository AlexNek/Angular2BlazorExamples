using static Netlify.ApiClient.Hero.SearchHeroesRequest;

namespace Netlify.ApiClient.Hero;

//public class SearchHeroesOptions
//{
//    public enum OrderDirection
//    {
//        Desc
//    }
//    public enum HeroOrderField
//    {
//        USERS_VOTED
//    }

//    public string Query { get; set; }
//    public string After { get; set; }
//    public int First { get; set; }
//    public OrderByParams OrderBy { get; set; }
//    public int Skip { get; set; }

//    public class OrderByParams
//    {
//        public SearchHeroesOptions.OrderDirection Direction { get; set; }
//        public SearchHeroesOptions.HeroOrderField Field { get; set; }
//    }
//}

public class SearchHeroesRequest
{
    public enum OrderDirection
    {
        Asc = 1,

        Desc = 2
    }
    public enum HeroOrderField
    {
        UsersVoted = 1
    }

    public string Query { get; set; }

    public string After { get; set; }

    public int First { get; set; }

    public OrderByParams OrderBy { get; set; }

    public int Skip { get; set; }

    public class OrderByParams
    {
        public OrderDirection Direction { get; set; }

        public HeroOrderField Field { get; set; }
    }
}