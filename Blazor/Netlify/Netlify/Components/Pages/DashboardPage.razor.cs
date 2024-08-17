using System.Reactive.Subjects;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.FluentUI.AspNetCore.Components;

using Netlifly.Shared;

using Netlify.ApiClient.Hero;
using Netlify.Components.Account.Components;
using Netlify.Helpers;

namespace Netlify.Components.Pages;

public partial class DashboardPage
{
    [Inject]
    private IHeroService HeroService { get; set; }

    [Inject]
    private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    private User? _user;

    private List<Hero> _publicHeroes = new List<Hero>();

    private readonly Subject<bool> destroy = new Subject<bool>();

    private IEnumerable<IdentityError>? _errors;

    private string? _accessToken;

    private string? Message =>
        _errors is null
            ? null
            : $"Error: {string.Join(", ", _errors.Select(error => error.Description))}";

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        _user = ClaimsHelper.CreateUser(user);
        _accessToken = ClaimsHelper.GetAccessToken(user);
        // AuthRepository.UserObservable
        //     .TakeUntil(destroy)
        //     .Subscribe(u =>
        //     {
        //         if (u != null)
        //         {
        //             user = u;
        //             CheckUserLanguage();
        //         }
        //     });

        await LoadPublicHeroes();
    }

    private async Task LoadPublicHeroes()
    {
        try
        {
            var heroes = await HeroService.SearchHeroesAsync(
                             new SearchHeroesRequest()
                                 {
                                     Query = "",
                                     After = "",
                                     First = 5,
                                     OrderBy = new SearchHeroesRequest.OrderByParams
                                                   {
                                                       Direction = SearchHeroesRequest.OrderDirection.Desc,
                                                       Field = SearchHeroesRequest.HeroOrderField.UsersVoted
                                                   },
                                     Skip = 0
                                 });

            if (heroes != null)
            {
                HandleResponse();
                List<Netlifly.Shared.Hero> heroList = heroes.Edges.Select(edge => edge.Node).ToList();
                _publicHeroes = heroList;
                StateHasChanged();
            }
        }
        catch (Exception ex)
        {
            HandleError(ex);
        }
    }

    private void HandleError(Exception ex)
    {
        var networkError = UtilService.CheckNetworkError(ex);
        if (networkError)
        {
            string stringLocalized = AlertId.NetworkError.ToStringLocalized(Localizer);
            _errors = [new IdentityError { Description = stringLocalized }];
            //ToastService.ShowError(stringLocalized);
        }
        else
        {
            //string stringLocalized = AlertId.GenericError.ToStringLocalized(Localizer);
            _errors = [new IdentityError { Description = ex.Message }];
            //ToastService.ShowError(stringLocalized);
        }
    }

    //private async Task VoteForHero(Hero hero)
    //{
    //    try
    //    {
    //        await HeroService.VoteForHeroAsync(hero.Id, TODO);
    //        await LoadPublicHeroes();
    //    }
    //    catch (Exception ex)
    //    {
    //        HandleError(ex);
    //    }
    //}

    private void HandleResponse()
    {
        //ToastService.ShowSuccess(AlertId.UserSaved.ToStringLocalized(Localizer));

        //_isProfileButtonLoading = false;
        //StateHasChanged();
    }

    //public void Dispose()
    //{
    //    destroy.OnNext(true);
    //    destroy.Dispose();
    //}
}
