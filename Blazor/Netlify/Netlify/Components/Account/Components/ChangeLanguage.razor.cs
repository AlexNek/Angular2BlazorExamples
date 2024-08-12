using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using Netlifly.Shared;
using Netlifly.Shared.Request;
using Netlify.ApiClient.Auth;
using Netlify.SharedResources;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Claims;

namespace Netlify.Components.Account.Components;

public partial class ChangeLanguage
{
    private IEnumerable<IdentityError>? _identityErrors;

    private bool _isProfileButtonLoading;

    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; }

    [Inject]
    private IAuthService ApiClient { get; set; }

    [Inject]
    private IJSRuntime JS { get; set; }

    //[Inject]
    //private IdentityRedirectManager RedirectManager { get; set; }

    [Inject]
    private NavigationManager Navigation { get; set; }

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    private CultureInfo[] supportedCultures = SharedLocalizerHelper.GetSupportedCultures();

    private bool _waitingTime;

    private string? Message =>
        _identityErrors is null
            ? null
            : $"Error: {string.Join(", ", _identityErrors.Select(error => error.Description))}";

    public async Task UpdateUser(EditContext editContext)
    {
        _waitingTime = true;
        try
        {
            User user = new User() { Id = Input.Id, Email = Input.Email, FirstName = Input.Name };
            string? language = Input.Culture?.TwoLetterISOLanguageName;
            UpdateUserData userData = new UpdateUserData { User = user, Language = language };
            var authUserData = await ApiClient.UpdateUserAsync(userData, Input.AccessToken);
            if (authUserData != null)
            {

                HandleResponse();
                await ApplySelectedCultureAsync(JS, language);

                // Trick to use HttpContext from parent
                var queryParameters = new Dictionary<string, object?> { { nameof(User.Language), language } };
                var newUri = Navigation.GetUriWithQueryParameters(Navigation.Uri, queryParameters);
                Navigation.NavigateTo(newUri);
            }
            else
            {
                _identityErrors = [new IdentityError { Description = "Update is not successful" }];
                if (Message != null)
                {
                    ToastService.ShowError(Message);
                }
            }
        }
        catch (Exception ex)
        {
            _identityErrors = [new IdentityError { Description = ex.Message }];
            HandleError(ex);
        }
        finally
        {
            _waitingTime = false;
        }
    }

    private async Task ApplySelectedCultureAsync(IJSRuntime js, string cultureName)
    {
        CultureInfo selectedCulture = new CultureInfo(cultureName);
        if (CultureInfo.CurrentCulture != selectedCulture)
        {
            await js.InvokeVoidAsync("blazorCulture.set", selectedCulture!.Name);

            var uri = new Uri(Navigation.Uri)
                .GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
            var cultureEscaped = Uri.EscapeDataString(selectedCulture.Name);
            var uriEscaped = Uri.EscapeDataString(uri);

            Navigation.NavigateTo(
                $"Culture/Set?culture={cultureEscaped}&redirectUri={uriEscaped}",
                forceLoad: true);
        }
    }
    private void HandleError(Exception error)
    {
        var networkError = UtilService.CheckNetworkError(error);
        if (networkError)
        {
            ToastService.ShowError(AlertId.NetworkError.ToStringLocalized(Localizer));
        }
        else
        {
            ToastService.ShowError(AlertId.UpdateUserError.ToStringLocalized(Localizer));
        }

        _isProfileButtonLoading = networkError;
        //StateHasChanged();
    }

    private void HandleResponse()
    {
        ToastService.ShowSuccess(AlertId.UserSaved.ToStringLocalized(Localizer));

        _isProfileButtonLoading = false;
        //StateHasChanged();
    }
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        //var user = ServerState.User;
        if (user.Identity is { IsAuthenticated: true })
        {
            Input.Id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Input.Email = user.FindFirst(ClaimTypes.Email)?.Value;
            Input.Name = user.Identity.Name;
            Input.AccessToken = user.FindFirst(AdditionalClaimTypes.AccessToken)?.Value;
            var language = user.FindFirst(ClaimTypes.Locality)?.Value;
            if (language != null)
            {
                Input.Culture = new CultureInfo(language);
            }
        }

        //bool isRunningOnWasm = IsRunningOnWasm();
    }
    private sealed class InputModel
    {
        //hidden
        public string? Email { get; set; } = string.Empty;

        //Hidden userId
        public string? Id { get; set; }

        //Hidden
        public string? AccessToken { get; set; }

        //hidden
        public string? Name { get; set; } = string.Empty;

        //[Required]
        //public string? Language { get; set; }

        [Required]
        public CultureInfo? Culture { get; set; }
    }
}
