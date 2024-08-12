using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Netlify.ApiClient.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using Netlifly.Shared;
using GraphQL;
using System.ComponentModel.DataAnnotations;

using Netlify.Helpers;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.FluentUI.AspNetCore.Components;
using Netlifly.Shared.Request;
using Netlifly.Shared.Response;

namespace Netlify.Components.Account.Pages;

public partial class RefreshTokens2 : ComponentBase
{
    private IEnumerable<IdentityError>? _identityErrors;
    
    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    private DateTime _accessTokenExpiry;

    private DateTime _refreshTokenExpiry;

    [Inject]
    private IAuthService ApiClient { get; set; }

    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; }

    [Inject]
    private NavigationManager Navigation { get; set; }

    private Color? _accessTokenColor = Color.Success;
    private Color? _refreshTokenColor = Color.Success;

    private string? Message =>
        _identityErrors is null
            ? null
            : $"Error: {string.Join(", ", _identityErrors.Select(error => error.Description))}";

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    private bool _waitingTime;
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
            Input.Email = user.FindFirst(ClaimTypes.Email)?.Value;
            string? userAccessToken = user.FindFirst(AdditionalClaimTypes.AccessToken)?.Value;
            string? userRefreshToken = user.FindFirst(AdditionalClaimTypes.RefreshToken)?.Value;
            Input.Name = user.Identity.Name;
            (_accessTokenExpiry, _refreshTokenExpiry) = TokenHelper.GetTokenExpirationsDate(userAccessToken, userRefreshToken);
            var tokenExpirationsState = TokenHelper.GetTokenExpirationsState(userAccessToken, userRefreshToken);
            if (tokenExpirationsState.IsAccessTokenExpired)
            {
                _accessTokenColor = Color.Error;
            }

            if (tokenExpirationsState.IsRefreshTokenExpired)
            {
                _refreshTokenColor = Color.Error;
            }

            //HttpContext.Session.SetString(AdditionalClaimTypes.RefreshToken, userRefreshToken); // Store token in session

        }

        //bool isRunningOnWasm = IsRunningOnWasm();
    }

    public async Task UpdateUser(EditContext editContext)
    {
        _waitingTime = true;
        try
        {

            //var refreshToken = HttpContext.Session.GetString(AdditionalClaimTypes.RefreshToken);

            // Delay navigation to ensure form processing is complete
            await Task.Yield();

            // Force a full reload to apply the cookie
            Navigation.NavigateTo("/secure-page");
            //if (string.IsNullOrEmpty(refreshToken))
            //{
            //    _identityErrors = [new IdentityError { Description = "Can not get access token for update" }];
            //    return;
            //}
            
            //var tokens = await ApiClient.RefreshTokenAsync(refreshToken);
            //if (tokens != null)
            //{

            //    bool isUpdated = await ClaimsHelper.UpdateTokens(HttpContext, tokens);
            //    if (isUpdated)
            //    {
            //        // Force a full reload to apply the cookie
            //        Navigation.NavigateTo("/secure-page", forceLoad: true);
            //    }
                
            //}
            //else
            //{
            //    _identityErrors = [new IdentityError { Description = "Update is not successful" }];

            //}
        }
        catch (Exception ex)
        {
            _identityErrors = [new IdentityError { Description = ex.Message }];
        }
        finally
        {
            _waitingTime = false;
        }
    }
    private sealed class InputModel
    {
       
        public string Email { get; set; } = string.Empty;

        public string? Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? AccessToken { get; set; }
    }
}