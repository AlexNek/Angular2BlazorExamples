using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Netlify.Controllers;

[Route("[controller]/[action]")]
public class CultureController : Controller
{
    public IActionResult Set(string culture, string redirectUri)
    {
        if (culture != null)
        {
            var cookieOptions = new CookieOptions
                                    {
                                        Expires = DateTimeOffset.UtcNow.AddYears(1) // Set a long expiration time
                                    };
            string cookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture, culture));
            HttpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                cookieValue,
                cookieOptions);
        }

        return LocalRedirect(redirectUri);
    }
}
