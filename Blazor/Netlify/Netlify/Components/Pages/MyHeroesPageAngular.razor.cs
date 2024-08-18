using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

using Netlifly.Shared;
using Netlify.Helpers;

namespace Netlify.Components.Pages
{
    public partial class MyHeroesPageAngular : ComponentBase
    {
        private HeroModal heroModal;

        private Hero heroSelected;

        private User? _user;

        private List<Hero> UserHeroes = new List<Hero>();

        private string? _accessToken;

        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            _user = ClaimsHelper.CreateUser(user);
            _accessToken = ClaimsHelper.GetAccessToken(user);
            await LoadUserHeroes(_accessToken);
        }

        private async Task DeleteHero(Hero hero)
        {
            await HeroService.DeleteHeroAsync(hero.Id);
            UserHeroes.Remove(hero);
            //lertService.Create(AlertId.HeroDeleted);
            StateHasChanged();
        }

        private async Task LoadUserHeroes(string accessToken)
        {
            var userWithHeroes = await UserService.GetMeAsync(accessToken);
            if (userWithHeroes != null)
            {
                UserHeroes = userWithHeroes.Heroes.ToList();
                //StateHasChanged();
            }
        }

        private void OpenHeroModal(Hero? hero = null)
        {
            heroSelected = hero ?? new Hero(null)
                                       {
                                           Id = string.Empty,
                                           RealName = string.Empty,
                                           AlterEgo = string.Empty,
                                           Image = "no-id",
                                           Public = false,
                                           UsersVoted = new List<Hero>().ToArray()
                                       };
            heroModal.Show();
        }
    }
}
