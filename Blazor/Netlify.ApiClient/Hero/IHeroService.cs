using Netlify.Components.Pages;

namespace Netlify.ApiClient.Hero;

public interface IHeroService
{
    Task<SearchHeroesResponse.SearchHeroesData?> SearchHeroesAsync(SearchHeroesRequest request);

    Task<Netlifly.Shared.Hero?> CreateHeroAsync(CreateHeroRequest data);

    Task<DeleteHeroResponse.DeleteHeroData?> DeleteHeroAsync(string heroId);

    Task<Netlifly.Shared.Hero?> VoteForHeroAsync(string heroId, string? accessToken);

    Task<int?> GetVoteForHeroAsync(string heroId);

    Task<Netlifly.Shared.Hero?> UpdateHeroAsync(UpdateHeroRequest data);
}
