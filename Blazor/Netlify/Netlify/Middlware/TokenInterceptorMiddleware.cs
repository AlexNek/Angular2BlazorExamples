using System.Net;
using System.Security.Claims;

using Microsoft.AspNetCore.Components;

using Netlifly.Shared;

using Netlify.ApiClient.Auth;
using Netlify.Helpers;

namespace Netlify.Middlware
{
    public class TokenInterceptorMiddleware
    {
        //private readonly IAuthRepository _authRepository;

        private readonly IAuthService _authService;

        private readonly string[] _excludedPaths =
            {
                "/health", "/Netlify.Client.styles.css", "/favicon.ico",
                "/_framework/blazor.web.js", "/_framework/dotnet.js", "/_framework/dotnet.js.map",
                "/_framework/blazor.boot.json", "/_framework/dotnet.runtime.js",
                "/_framework/dotnet.native.js", "/_framework/dotnet.runtime.js.map",
                "/_framework/blazor.web.js", "/_framework/dotnet.js",
                "/_framework/blazor.boot.json", "/_blazor/disconnect", "/auth/log-in",
                "/_blazor/negotiate", "/_blazor",
                "/weather"
            };

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
            // Skip token processing for excluded paths
            if (ShouldSkipTokenProcessing(context.Request.Path))
            {
                await _next(context);
                return;
            }
            _logger.LogDebug("Middleware processing path: {Path}", context.Request.Path);

            string? accessToken = null; //_authRepository.GetAccessToken();
            string? refreshToken = null; //_authRepository.GetRefreshToken();

            var user = context.User;
            if (user.Identity is { IsAuthenticated: true })
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var InputEmail = user.FindFirst(ClaimTypes.Email)?.Value;
                var InputName = user.Identity.Name;
                accessToken = user.FindFirst(AdditionalClaimTypes.AccessToken)?.Value;
                refreshToken = user.FindFirst(AdditionalClaimTypes.RefreshToken)?.Value;
            }

            if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                var tokenValidation =
                    TokenHelper.GetTokenExpirationsState(accessToken, refreshToken);
                //// TEST
                //NavigateToLogout(context);
                //await _next(context);
                //return;

                if (tokenValidation.IsAccessTokenExpired)
                {
                    if (!tokenValidation.IsRefreshTokenExpired)
                    {
                        var tokens = await _authService.RefreshTokenAsync(refreshToken);
                        if (!string.IsNullOrEmpty(tokens?.AccessToken))
                        {
                            context.Request.Headers["Authorization"] =
                                $"Bearer {tokens.AccessToken}";

                            bool isUpdated = await ClaimsHelper.UpdateTokens(context, tokens);
                            if (isUpdated)
                            {
                                //TODO: Check expiration in client too, client need full page refresh
                            }
                        }
                        else
                        {
                            NavigateToLogout(context);
                            await _next(context);
                            return;
                        }
                    }
                    else
                    {
                        NavigateToLogout(context);
                        await _next(context);
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
                if (!context.Response.HasStarted) // Only check if response isn’t already streaming
                {
                    CheckUnauthorizedError(context);
                }
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
                // Check if this is a streaming endpoint
                var endpoint = context.GetEndpoint();
                if (endpoint?.Metadata.GetMetadata<StreamRenderingAttribute>() != null)
                {
                    // Log and skip redirect for streaming endpoints
                    _logger.LogWarning("Unauthorized detected on streaming endpoint, skipping redirect.");
                    return;
                }
                NavigateToLogout(context);
            }
        }

        private void NavigateToLogout(HttpContext context)
        {
            context.Response.Redirect(
                $"/auth/logout?origin={WebUtility.UrlEncode(context.Request.Path)}&alertId=SessionExpired");
        }

        private bool ShouldSkipTokenProcessing(PathString path)
        {
            return _excludedPaths.Any(
                excludedPath =>
                    path.StartsWithSegments(excludedPath, StringComparison.OrdinalIgnoreCase));
        }
    }
}
