using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

using Netlify.ApiClient.Auth;
using static Netlify.Components.Account.Pages.Register;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;
using Netlifly.Shared.Request;

using Netlify.SharedResources;
using Microsoft.Extensions.Localization;

using Netlify.Attributes;
using Microsoft.FluentUI.AspNetCore.Components;

using Netlifly.Shared.Response;
using Netlifly.Shared;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Netlify.Components.Account.Components;

public partial class ChangePassword
{
    private IEnumerable<IdentityError>? _identityErrors;

    [Inject]
    private IAuthService ApiClient { get; set; }
    
    [Inject] 
    private IToastService ToastService { get; set;}

    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; }

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    //[CascadingParameter]
    //private HttpContext HttpContext { get; set; } = default!;

    [Inject]
    private IHttpContextAccessor HttpContextAccessor { get; set; }

    private string? Message =>
        _identityErrors is null
            ? null
            : $"Error: {string.Join(", ", _identityErrors.Select(error => error.Description))}";

    private bool _waitingTime;

    private bool _isProfileButtonLoading;

    public async Task UpdatePassword(EditContext editContext)
    {
        try
        {
            _waitingTime = true;
            var acccessToken = HttpContextAccessor.HttpContext.Session.GetString(AdditionalClaimTypes.AccessToken);

            if (string.IsNullOrEmpty(acccessToken))
            {
                _identityErrors = [new IdentityError { Description = "Error:Can not get access token for update" }];
                return;
            }

            var okData = await ApiClient.ChangePasswordAsync(Input.Password, Input.NewPassword, Input.Id, acccessToken);
            if (okData != null)
            {
                HandleResponse();
            }
            else
            {
                ToastService.ShowError(AlertId.CurrentPasswordError.ToStringLocalized(Localizer));
                _identityErrors = [new IdentityError() { Description = "Change password is not successful" }];
            }

            //var authUserData = await apiClient.SignupAsync(registerData);
            //if (authUserData != null)
            //{
            //    //RedirectManager.RedirectTo(ReturnUrl);
            //}
            //else
            //{
            //    identityErrors = [new IdentityError() { Description = "Registration is not successful" }];
            //}
        }
        catch (Exception ex)
        {
            HandleError(ex);
        }
        finally
        {
            _waitingTime=false;
        }
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
            
            string? userAccessToken = user.FindFirst(AdditionalClaimTypes.AccessToken)?.Value;
            HttpContextAccessor.HttpContext.Session.SetString(
                AdditionalClaimTypes.AccessToken,
                userAccessToken); // Store token in session
        }

        //bool isRunningOnWasm = IsRunningOnWasm();
    }
    private void HandleError(Exception ex)
    {
        _identityErrors = [new IdentityError() { Description = ex.Message }];
        var networkError = UtilService.CheckNetworkError(ex);
        if (networkError)
        {
            ToastService.ShowError(AlertId.NetworkError.ToStringLocalized(Localizer));
        }
        else
        {
            //var registerErrors = error.GraphQLErrors;
            //if (registerErrors.Any())
            {
                ToastService.ShowError(AlertId.CurrentPasswordError.ToStringLocalized(Localizer));
            }
        }

        _isProfileButtonLoading = networkError;
        //StateHasChanged();
    }

    private void HandleResponse()
    {
        ToastService.ShowSuccess(AlertId.PasswordChanged.ToStringLocalized(Localizer));

        _isProfileButtonLoading = false;
        //StateHasChanged();
    }

    private sealed class InputModel
    {
        //Hidden userId
        public string? Id { get; set; }

        [Required]
        [PasswordLengthDefaultLoc]
        [DataType(DataType.Password)]
        [RegularExpressionLoc(PasswordRegexPattern,  "PasswordMustContainLetterAndNumber")]
        public string Password { get; set; } = "";


        [Required]
        [DataType(DataType.Password)]
        [PasswordLengthDefaultLoc]
        [Display(Name = "Confirm password")]
        [CompareNotEqualLoc(nameof(Password),  "The new password must be different from the old password.")]
        public string NewPassword { get; set; } = "";
        
    }
}
