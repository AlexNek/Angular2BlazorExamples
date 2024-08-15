using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using Netlifly.Shared;
using Netlifly.Shared.Response;

namespace Netlify.Helpers
{
    public class ClaimsHelper
    {
        public const int ExpirationDays = 7;

        public static List<Claim> CreateNewClaims(InternalLoginRequest internalLogin, AuthUserData userData)
        {
            return new List<Claim>
                       {
                           new Claim(ClaimTypes.Name, userData.User.FirstName),
                           new Claim(ClaimTypes.Email, internalLogin.Email),
                           new Claim(ClaimTypes.Role, "User"),
                           new Claim(ClaimTypes.Locality, userData.User.Language),
                           new Claim(ClaimTypes.NameIdentifier, userData.User.Id),
                           new Claim(AdditionalClaimTypes.AccessToken, userData.AccessToken),
                           new Claim(AdditionalClaimTypes.RefreshToken, userData.RefreshToken),
                           new Claim(AdditionalClaimTypes.IsPersistentClaim, internalLogin.RememberMe.ToString()),
                           new Claim(AdditionalClaimTypes.ExpirationDaysClaim, ExpirationDays.ToString()),
                       };
        }

        public static User CreateUser(ClaimsPrincipal user)
        {
            var ret = new User();

            if (user.Identity is { IsAuthenticated: true })
            {
                ret.Id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                ret.Email = user.FindFirst(ClaimTypes.Email)?.Value;

                ret.FirstName = user.Identity.Name;
            }

            return ret;
        }

        public static async Task Login(
            HttpContext httpContext,
            InternalLoginRequest internalLogin,
            AuthUserData userData)
        {
            var claims = CreateNewClaims(internalLogin, userData);

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
                                     {
                                         IsPersistent = internalLogin.RememberMe,
                                         ExpiresUtc = DateTimeOffset.UtcNow.AddDays(ExpirationDays)
                                     };

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);
        }

        public static async Task<bool> UpdateTokens(HttpContext context, UpdateTokenData tokens)
        {
            bool ret = false;
            var user = context.User;
            if (user.Identity is { IsAuthenticated: true })
            {
                // Update the claims with the new token
                if (context.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    var newClaims = new List<Claim>(claimsIdentity.Claims);
                    newClaims.RemoveAll(c => c.Type == AdditionalClaimTypes.AccessToken);
                    newClaims.RemoveAll(c => c.Type == AdditionalClaimTypes.RefreshToken);
                    newClaims.Add(new Claim(AdditionalClaimTypes.AccessToken, tokens?.AccessToken));
                    newClaims.Add(new Claim(AdditionalClaimTypes.RefreshToken, tokens?.RefreshToken));

                    // Create a new ClaimsIdentity and ClaimsPrincipal with the updated claims
                    var newIdentity = new ClaimsIdentity(newClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var newPrincipal = new ClaimsPrincipal(newIdentity);

                    AuthenticationProperties authProperties = GetAuthenticationProperties(user);

                    // Sign in with the new principal
                    await context.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        newPrincipal,
                        authProperties);

                    ret = true;
                }
            }

            return ret;
        }

        public static async Task<bool> UpdateUserLanguageAsync(HttpContext context, string userLanguage)
        {
            bool ret = false;
            var user = context.User;
            if (user.Identity is { IsAuthenticated: true } && !string.IsNullOrEmpty(userLanguage))
            {
                // Update the claims with the new token
                if (context.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    var newClaims = new List<Claim>(claimsIdentity.Claims);
                    newClaims.RemoveAll(c => c.Type == ClaimTypes.Locality);
                    newClaims.Add(new Claim(ClaimTypes.Locality, userLanguage));

                    // Create a new ClaimsIdentity and ClaimsPrincipal with the updated claims
                    var newIdentity = new ClaimsIdentity(newClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var newPrincipal = new ClaimsPrincipal(newIdentity);

                    AuthenticationProperties authProperties = GetAuthenticationProperties(user);

                    // Sign in with the new principal
                    await context.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        newPrincipal,
                        authProperties);

                    ret = true;
                }
            }

            return ret;
        }

        public static async Task<bool> UpdateUserNameAsync(HttpContext context, string userName)
        {
            bool ret = false;
            var user = context.User;
            if (user.Identity is { IsAuthenticated: true } && !string.IsNullOrEmpty(userName))
            {
                // Update the claims with the new token
                if (context.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    var newClaims = new List<Claim>(claimsIdentity.Claims);
                    newClaims.RemoveAll(c => c.Type == ClaimTypes.Name);
                    newClaims.Add(new Claim(ClaimTypes.Name, userName));

                    // Create a new ClaimsIdentity and ClaimsPrincipal with the updated claims
                    var newIdentity = new ClaimsIdentity(newClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var newPrincipal = new ClaimsPrincipal(newIdentity);

                    AuthenticationProperties authProperties = GetAuthenticationProperties(user);

                    // Sign in with the new principal
                    await context.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        newPrincipal,
                        authProperties);

                    ret = true;
                }
            }

            return ret;
        }

        private static AuthenticationProperties GetAuthenticationProperties(ClaimsPrincipal user)
        {
            var isPersistentClaim = user.FindFirst(AdditionalClaimTypes.IsPersistentClaim)?.Value;

            bool previousIsPersistent = false;
            if (bool.TryParse(isPersistentClaim, out var isPersistent))
            {
                previousIsPersistent = isPersistent;
            }

            var expirationDays = user.FindFirst(AdditionalClaimTypes.ExpirationDaysClaim)?.Value;
            int previousExpirationDays = 1;
            if (int.TryParse(expirationDays, out var expirationDaysTemp))
            {
                previousExpirationDays = expirationDaysTemp;
            }

            var authProperties = new AuthenticationProperties
                                     {
                                         IsPersistent = previousIsPersistent,
                                         ExpiresUtc = DateTimeOffset.UtcNow.AddDays(previousExpirationDays)
                                     };
            return authProperties;
        }
    }
}
