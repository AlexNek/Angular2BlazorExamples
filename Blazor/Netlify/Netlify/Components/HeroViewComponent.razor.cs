using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.FluentUI.AspNetCore.Components;

using Netlifly.Shared;

using Netlify.ApiClient.Hero;
using Netlify.Components.Account.Components;

using Newtonsoft.Json;

namespace Netlify.Components;

public partial class HeroViewComponent
{
    [Parameter]
    public string? HeroAsString { get; set; }

    [Parameter]
    public string? AccessToken { get; set; }

    [Inject]
    private IHeroService HeroService { get; set; }

    [Inject]
    private IToastService ToastService { get; set; }

    private Hero? Hero { get; set; }

    private IEnumerable<IdentityError>? _errors;
    private IEnumerable<IdentityError>? _info;

    private string? Message
    {
        get
        {
            string? infoText = null;

            if (_info != null)
            {
                infoText = string.Join(", ", _info.Select(info => info.Description));
            }

            return _errors is null
                       ? infoText
                       : $"Errors: {string.Join(", ", _errors.Select(error => error.Description))}";
        }
    }

    private async Task VoteForHero()
    {
        _info = null;
        _errors = null;
        if (Hero == null)
        {
            throw new ArgumentNullException(nameof(HeroAsString));
        }

        try
        {
            if (Hero != null && !string.IsNullOrEmpty(AccessToken))
            {
                //response doesn't contain votes
                var hero = await HeroService.VoteForHeroAsync(Hero.Id, AccessToken);
                if (hero != null)
                {
                    int? votesForHero = await HeroService.GetVoteForHeroAsync(hero.Id);
                    if (votesForHero != null)
                    {
                        Hero.UsersVoted = new Hero[votesForHero.Value];
                    }
                    
                }

                //Hero.UsersVoted;
                ToastService.ShowSuccess("Your vote received");
                StateHasChanged();
            }
            else
            {
                _errors = [new IdentityError { Description = "wrong parameters" }];
            }
            //await LoadPublicHeroes();
        }
        catch (Exception ex)
        {
            HandleError(ex);
        }
    }

    /// <summary>
    /// Method invoked when the component has received parameters from its parent in
    /// the render tree, and the incoming values have been assigned to properties.
    /// </summary>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        if (!string.IsNullOrEmpty(HeroAsString))
        {
            Hero = JsonConvert.DeserializeObject<Hero>(HeroAsString);
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
}
