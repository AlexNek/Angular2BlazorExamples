using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using Netlifly.Shared;
using Netlifly.Shared.Response;

using Netlify.ApiClient.Auth;

namespace Netlify.Middlware
{
    public class TokenInterceptorMiddleware
    {
        //private readonly IAuthRepository _authRepository;

        private readonly IAuthService _authService;

        private readonly ILogger<TokenInterceptorMiddleware> _logger;

        private readonly RequestDelegate _next;

        public TokenInterceptorMiddleware(
            RequestDelegate next,
            ILogger<TokenInterceptorMiddleware> logger,
            IAuthService authService
            //IAuthRepository authRepository
            )
        {
            _next = next;
            _logger = logger;
            _authService = authService;
            //_authRepository = authRepository;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string? accessToken = null; //_authRepository.GetAccessToken();
            string? refreshToken = null; //_authRepository.GetRefreshToken();

            var user = context.User;
            if (user.Identity is { IsAuthenticated: true })
            {
               var InputId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
               var InputEmail = user.FindFirst(ClaimTypes.Email)?.Value;
               var InputName = user.Identity.Name;
               accessToken = user.FindFirst(AdditionalClaimTypes.AccessToken)?.Value;
               refreshToken = user.FindFirst(AdditionalClaimTypes.RefreshToken)?.Value;
            }
            if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                var tokenValidation = GetTokenExpirations(accessToken, refreshToken);

                if (tokenValidation.isAccessTokenExpired)
                {
                    if (!tokenValidation.isRefreshTokenExpired)
                    {
                        var tokens = await _authService.RefreshTokenAsync(refreshToken);
                        if (!string.IsNullOrEmpty(tokens?.AccessToken))
                        {
                            context.Request.Headers["Authorization"] = $"Bearer {tokens.AccessToken}";

                            bool isUpdated = await ClaimsHelper.UpdateTokens(context, tokens);
                            if (isUpdated)
                            {
                                //TODO: Check expiration in client too, client need full page refresh
                            }
                        }
                        else
                        {
                            NavigateToLogout(context);
                            return;
                        }
                    }
                    else
                    {
                        NavigateToLogout(context);
                        return;
                    }
                }
                else
                {
                    context.Request.Headers["Authorization"] = $"Bearer {accessToken}";
                }
            }

            try
            {
                await _next(context);
                CheckUnauthorizedError(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during request processing.");
                throw;
            }
        }

        private void CheckUnauthorizedError(HttpContext context)
        {
            if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                NavigateToLogout(context);
            }
        }

        private (bool isAccessTokenExpired, bool isRefreshTokenExpired) GetTokenExpirations(
            string accessToken,
            string refreshToken)
        {
            var accessTokenExpiry = GetTokenExpiry(accessToken);
            var refreshTokenExpiry = GetTokenExpiry(refreshToken);

            var isAccessTokenExpired = DateTime.UtcNow >= accessTokenExpiry;
            var isRefreshTokenExpired = DateTime.UtcNow >= refreshTokenExpiry;

            return (isAccessTokenExpired, isRefreshTokenExpired);
        }

        private DateTime GetTokenExpiry(string token)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
            var expiry = jwtToken?.ValidTo;
            return expiry ?? DateTime.MinValue;
        }

        private void NavigateToLogout(HttpContext context)
        {
            context.Response.Redirect(
                $"/logout?origin={WebUtility.UrlEncode(context.Request.Path)}&alertId=SESSION_EXPIRED");
        }
    }
}
