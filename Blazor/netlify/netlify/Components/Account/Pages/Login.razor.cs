using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;

using Netlifly.Shared;

namespace Netlify.Components.Account.Pages
{
    public partial class Login
    {
        private string? errorMessage;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromForm(FormName = "login")]
        private InputModel Input { get; set; } = new();

        [SupplyParameterFromQuery]
        private bool? IsDemoLogin { get; set; }

        [Inject]
        private ILogger<Login> Logger { get; set; }

        private string? ReturnUrl { get; set; }

        public async Task LoginUser()
        {
            // Serialize the login request
            var internalLogin = new InternalLoginRequest
                                    {
                                        Username = "NoName",
                                        Email = Input.EMail,
                                        Password = Input.Password,
                                        RememberMe = Input.RememberMe,
                                        Locale = CultureInfo.CurrentCulture.Name
                                    };

            var response = LoginWithCookie(internalLogin);
            if (response)
            {
                // Force a full reload to apply the cookie
                NavigationManager.NavigateTo("/secure-page", forceLoad: true);
                //RedirectManager.RedirectTo(ReturnUrl, true);
            }
            else
            {
                // Handle unsuccessful login
                //var responseBody = await response.Content.ReadAsStringAsync();
                string responseBody = "";
                errorMessage = $"Error: Invalid login attempt. {responseBody}";
            }
        }

        protected override async Task OnInitializedAsync()
        {
            ReturnUrl = "/";
            if (HttpContext != null)
            {
                if (HttpMethods.IsGet(HttpContext.Request.Method))
                {
                    // Clear the existing external cookie to ensure a clean login process
                    //await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                }
            }

            if (IsDemoLogin.HasValue && IsDemoLogin.Value)
            {
                Input.EMail = "test@gmail.com";
                Input.Password = "Admin1";
            }
        }

        private sealed class InputModel
        {
            [Required]
            [EmailAddress]
            public string EMail { get; set; } = "";

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = "";

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        private bool LoginWithCookie(InternalLoginRequest internalLogin)
        {
            if (internalLogin.Email == "test@gmail.com"
                && internalLogin.Password == "Admin1") // Replace with real user validation
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
                HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal,
                    authProperties);

                return true;
            }

            return false;
        }
    }
}
