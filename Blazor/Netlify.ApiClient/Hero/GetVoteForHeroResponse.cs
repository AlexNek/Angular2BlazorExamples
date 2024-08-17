namespace Netlify.ApiClient.Hero;

public class GetVoteForHeroResponse
{
    public HeroVotesData HeroVotes { get; set; }

    public class HeroVotesData
    {
        public int Votes { get; set; }
    }
}
