namespace Netlify.ApiClient.Hero;

public class DeleteHeroResponse
{
    public DeleteHeroData RemoveHero { get; set; }

    public class DeleteHeroData
    {
        public string Id { get; set; }
    }
}
