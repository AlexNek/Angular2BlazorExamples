using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;

using Netlifly.Shared;
using Netlifly.Shared.Request;

using Netlify.ApiClient.Auth;

namespace Netlify.Components.Account.Pages
{
    public partial class Login
    {
        private string? _errorMessage;

        [Inject]
        private IAuthService apiClient { get; set; }

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
            _errorMessage = null;

            // Serialize the login request
            var internalLogin = new InternalLoginRequest
                                    {
                                        Username = "NoName",
                                        Email = Input.EMail,
                                        Password = Input.Password,
                                        RememberMe = Input.RememberMe,
                                        Locale = CultureInfo.CurrentCulture.Name
                                    };

            var response = await LoginWithCookieAsync(internalLogin);
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
                _errorMessage = $"Error: Invalid login attempt. {responseBody}";
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
                Input.EMail = "demo@yahoo.com";
                Input.Password = "Demo1Demo1";
            }
        }

        private async Task<bool> LoginWithCookieAsync(InternalLoginRequest internalLogin)
        {
            AuthUserData? userData = null;
            try
            {
                userData = await apiClient.LogInAsync(internalLogin.Email, internalLogin.Password);
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                return false;
            }

            //&& internalLogin.Email == "test@gmail.com" && internalLogin.Password == "Admin1"
            if (userData!= null ) // Replace with real user validation
            {
                var claims = new List<Claim>
                                 {
                                     new Claim(ClaimTypes.Name, userData.User.Firstname),
                                     new Claim(ClaimTypes.Email, internalLogin.Email),
                                     new Claim(ClaimTypes.Role, "User"),
                                     new Claim(ClaimTypes.Locality, userData.User.Language)
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
    }
}
