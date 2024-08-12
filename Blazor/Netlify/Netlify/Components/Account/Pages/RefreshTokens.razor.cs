using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.FluentUI.AspNetCore.Components;

using Netlifly.Shared;

using Netlify.ApiClient.Auth;
using Netlify.Helpers;

namespace Netlify.Components.Account.Pages
{
    public partial class RefreshTokens
    {
        private Color? _accessTokenColor = Color.Success;

        private DateTime _accessTokenExpiry;

        private string? _errorMessage;

        private IEnumerable<IdentityError>? _identityErrors;

        private Color? _refreshTokenColor = Color.Success;

        private DateTime _refreshTokenExpiry;

        private bool _waitingTime;

        [Inject]
        private IAuthService apiClient { get; set; }

        [Inject]
        private IAuthService ApiClient { get; set; }

        [Inject]
        private AuthenticationStateProvider AuthStateProvider { get; set; }

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromForm(FormName = "refresh-tokens")]
        private InputModel Input { get; set; } = new();

        [Inject]
        private ILogger<Login> Logger { get; set; }

        private string? Message =>
            _identityErrors is null
                ? null
                : $"Error: {string.Join(", ", _identityErrors.Select(error => error.Description))}";

        public async Task UpdateTokens()
        {
            _waitingTime = true;
            bool isActionOk = false;

            try
            {
                var refreshToken = HttpContext.Session.GetString(AdditionalClaimTypes.RefreshToken);

                if (string.IsNullOrEmpty(refreshToken))
                {
                    _identityErrors = [new IdentityError { Description = "Error:Can not get access token for update" }];
                    return;
                }

                var tokens = await ApiClient.RefreshTokenAsync(refreshToken);
                if (tokens != null)
                {
                    bool isUpdated = await ClaimsHelper.UpdateTokens(HttpContext, tokens);
                    if (isUpdated)
                    {
                        isActionOk = true;
                    }
                }
                else
                {
                    _identityErrors = [new IdentityError { Description = "Error:Update is not successful" }];
                }
            }
            catch (Exception ex)
            {
                _identityErrors = [new IdentityError { Description = "Error: " + ex.Message }];
            }
            finally
            {
                _waitingTime = false;
            }

            if (isActionOk)
            {
                // for some reason navigation doesn't work inside try/catch block
                NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            Input = new();
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            //var user = ServerState.User;
            if (user.Identity is { IsAuthenticated: true })
            {
                Input.Id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Input.EMail = user.FindFirst(ClaimTypes.Email)?.Value;
                string? userAccessToken = user.FindFirst(AdditionalClaimTypes.AccessToken)?.Value;
                string? userRefreshToken = user.FindFirst(AdditionalClaimTypes.RefreshToken)?.Value;
                Input.Name = user.Identity.Name;
                (_accessTokenExpiry, _refreshTokenExpiry) =
                    TokenHelper.GetTokenExpirationsDate(userAccessToken, userRefreshToken);
                var tokenExpirationsState = TokenHelper.GetTokenExpirationsState(userAccessToken, userRefreshToken);
                if (tokenExpirationsState.IsAccessTokenExpired)
                {
                    _accessTokenColor = Color.Error;
                }

                if (tokenExpirationsState.IsRefreshTokenExpired)
                {
                    _refreshTokenColor = Color.Error;
                }

                HttpContext.Session.SetString(
                    AdditionalClaimTypes.RefreshToken,
                    userRefreshToken); // Store token in session
            }
        }

        private sealed class InputModel
        {
            //[Required]
            [EmailAddress]
            public string EMail { get; set; } = "";

            public string Id { get; set; }

            public string? Name { get; set; }

            //[Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = "";

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }
    }
}
