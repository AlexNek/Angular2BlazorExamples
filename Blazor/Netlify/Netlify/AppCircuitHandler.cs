using Microsoft.AspNetCore.Components.Server.Circuits;
using System.Security.Claims;

namespace Netlify
{
    internal class AppCircuitHandler : CircuitHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly InMemoryUserStateService _userStateService;

        public AppCircuitHandler(IHttpContextAccessor httpContextAccessor, InMemoryUserStateService userStateService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userStateService = userStateService;
        }

        public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null && httpContext.User.Identity is { IsAuthenticated: true })
            {
                var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != null)
                {
                    // Store the user state
                    _userStateService.SetUserState(userId, new UserState { UserId = userId });
                }
            }

            return Task.CompletedTask;
        }

        public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != null)
                {
                    // Clean up user state
                    _userStateService.RemoveUserState(userId);
                }
            }

            return Task.CompletedTask;
        }
    }


}
