namespace Netlify.ApiClient.Hero;

public interface IHeroService
{
    Task<SearchHeroesResponse.SearchHeroesData?> SearchHeroesAsync(SearchHeroesRequest request);

    Task<Netlifly.Shared.Hero?> CreateHero(CreateHeroData data);

    Task<DeleteHeroResponse.DeleteHeroData?> DeleteHero(string heroId);

    Task<Netlifly.Shared.Hero?> VoteForHeroAsync(string heroId, string? accessToken);

    Task<int?> GetVoteForHeroAsync(string heroId);
}
