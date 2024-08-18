using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.FluentUI.AspNetCore.Components;

using Netlifly.Shared;

using Netlify.ApiClient.Hero;
using Netlify.Helpers;

namespace Netlify.Components.Pages;

public partial class MyHeroesPage : ComponentBase
{
    //private IQueryable<Hero>? _userHeroesForGrid;
    private List<Hero>? _userHeroes = new List<Hero>();

    private Hero _editingHero = new Hero();

    private bool _isModalHidden = true;

    private string? _accessToken;

    private User _user;

    private bool _isEditing = false;

    private string? _message;

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

    private async Task LoadUserHeroes(string accessToken)
    {
        var userWithHeroes = await UserService.GetMeAsync(accessToken);
        if (userWithHeroes is { Heroes: not null })
        {
            _userHeroes = userWithHeroes.Heroes.ToList();
        }
        //StateHasChanged();
    }
    private void OnDialogResult(DialogResult obj)
    {
    }

    private void OpenNewHeroModal()
    {
        _editingHero = new Hero();
        _isModalHidden = false;
        _isEditing = false;
    }

    private void EditHero(Hero hero)
    {
        _editingHero = new Hero { Id = hero.Id, AlterEgo = hero.AlterEgo, RealName = hero.RealName };
        _isModalHidden = false;
        _isEditing = true;
    }

    /// <summary>
    /// Saves the hero information. If the hero's ID is empty, creates a new hero using the provided information. 
    /// Otherwise, updates the existing hero with the provided information.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task SaveHero()
    {
        _message = null;
        try
        {
            //If the EditingHero's ID is empty, creates a new hero
            if (string.IsNullOrEmpty(_editingHero.Id))
            {
                var newHeroRequest = new CreateHeroRequest
                                         {
                                             AlterEgo = _editingHero.AlterEgo, RealName = _editingHero.RealName
                                         };

                Hero? createdHero = await HeroService.CreateHeroAsync(newHeroRequest);
                if (createdHero != null)
                {
                    _userHeroes.Add(_editingHero);
                    CloseModal();
                }
            }
            else
            {
                var hero = _userHeroes.FirstOrDefault(h => h.Id == _editingHero.Id);
                if (hero != null)
                {
                    hero.AlterEgo = _editingHero.AlterEgo;
                    hero.RealName = _editingHero.RealName;
                    // Update existing hero
                    var updateHeroRequest = new UpdateHeroRequest
                                                {
                                                    Id = _editingHero.Id,
                                                    AlterEgo = _editingHero.AlterEgo,
                                                    RealName = _editingHero.RealName
                                                };
                    Hero? updatedHero = await HeroService.UpdateHeroAsync(updateHeroRequest);
                }
            }

           
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            _message = "Error: " + ex.Message;
        }
        finally
        {
            CloseModal();
        }
       
    }

    private async Task DeleteHero(Hero hero)
    {
        _message = null;
        _userHeroes.Remove(hero);

        try
        {
            // Call service to delete the hero
            await HeroService.DeleteHeroAsync(hero.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            _message = "Error: " + ex.Message;
        }
    }

    private void CloseModal()
    {
        _isModalHidden = true;
    }
}