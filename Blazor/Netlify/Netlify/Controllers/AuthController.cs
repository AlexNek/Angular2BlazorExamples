using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Netlifly.Shared;

namespace Netlify.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly ManualCookieHandler _cookieHandler;

    private readonly ILogger<AuthController> _logger;

    public AuthController(IHttpContextAccessor httpContextAccessor, ManualCookieHandler cookieHandler, ILogger<AuthController> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _cookieHandler = cookieHandler;
        _logger = logger;
    }

    //public AuthController( ManualCookieHandler cookieHandler, ILogger<AuthController> logger)
    //{
    //    _cookieHandler = cookieHandler;
    //    _logger = logger;
    //}

    //private const string ClaimTypesLocale = "locale";

    [HttpPost("login")]
    public IActionResult Login([FromBody] InternalLoginRequest internalLogin)
    {
        _logger.LogDebug("API Login start");
        if (internalLogin.Email == "test@gmail.com" && internalLogin.Password == "Admin1") // Replace with real user validation
        {
            var claims = new List<Claim>
                             {
                                 new Claim(ClaimTypes.Name, internalLogin.Username),
                                 new Claim(ClaimTypes.Email, internalLogin.Email),
                                 new Claim(ClaimTypes.Role, "User"),
                                 new Claim(ClaimTypes.Locality, internalLogin.Locale) 
                             };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
                                     {
                                         IsPersistent = internalLogin.RememberMe,
                                         ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
            //_cookieHandler.WriteAuthenticationCookieAsync(HttpContext,
            //    CookieAuthenticationDefaults.AuthenticationScheme,
            //    claimsPrincipal,
            //    authProperties);
            var cookieOptions = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(1), Secure = true };
            Response.Cookies.Append("AlexTest", "Test value", cookieOptions);
            _logger.LogDebug("API Login end");

            return Ok();
            //return LocalRedirect("/secure-page");
        }

        return Unauthorized();
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok();
    }

    //[Authorize]
    [AllowAnonymous]
    [HttpGet("user")]
    public IActionResult GetUser()
    {
        var user = _httpContextAccessor.HttpContext.User;
        if (user.Identity is { IsAuthenticated: true })
        {
            InternalLoginResponse userInfo = new()
                                                 {
                                                     Name = user.Identity?.Name,
                                                     IsAuthenticated = true,
                                                     //Claims = user.Claims.Select(c => new InternalLoginResponse.ClaimInfo() { c.Type, c.Value }).ToList()
                                                     Claims = user.Claims.Select(
                                                         c => new InternalLoginResponse.ClaimInfo
                                                                  {
                                                                      Type = c.Type, Value = c.Value
                                                                  }).ToList()
                                                     //Claims = user.Claims.ToList()
                                                 };

            return Ok(userInfo);
        }

        return Unauthorized();
    }
}
