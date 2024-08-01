using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace Netlify;

public class ManualCookieHandler
{
    private readonly CookieAuthenticationOptions _cookieOptions;

    public ManualCookieHandler(IOptions<CookieAuthenticationOptions> cookieOptions)
    {
        
        _cookieOptions = cookieOptions.Value;
    }
    
    public async Task WriteAuthenticationCookieAsync(HttpContext httpContext, string? scheme, ClaimsPrincipal principal, AuthenticationProperties? authProperties)
    {

        // Serialize the authentication ticket
        var ticket = new AuthenticationTicket(principal, authProperties, scheme);
        var serializedTicket = SerializeTicket(ticket);

        var cookieValue = Convert.ToBase64String(serializedTicket);

        httpContext.Response.Cookies.Append(_cookieOptions.Cookie.Name, cookieValue, new CookieOptions
            {
                Path = _cookieOptions.Cookie.Path,
                Domain = _cookieOptions.Cookie.Domain,
                HttpOnly = _cookieOptions.Cookie.HttpOnly,
                Secure = _cookieOptions.Cookie.SecurePolicy == CookieSecurePolicy.Always,
                SameSite = _cookieOptions.Cookie.SameSite
            });

        await Task.CompletedTask;
    }

    private byte[] SerializeTicket(AuthenticationTicket ticket)
    {
        var serializer = new TicketSerializer();
        return serializer.Serialize(ticket);
    }
}