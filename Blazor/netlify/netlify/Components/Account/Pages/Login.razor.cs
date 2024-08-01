using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using Netlifly.Shared;
using Newtonsoft.Json;

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

        private string? ReturnUrl { get; set; }
        [Inject]
        private ILogger<Login> Logger { get; set; }

        [Inject]
        private IHttpContextAccessor HttpContextAccessor { get; set; }

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

        public async Task LoginUser()
        {
            Logger.LogDebug("Form post login-start");
            //var httpClient = HttpClientFactory.CreateClient(nameof(CustomAuthStateProvider));

            // Serialize the login request
            var internalLogin = new InternalLoginRequest()
                                                     {
                                                         Username = "NoName",
                                                         Email = Input.EMail,
                                                         Password = Input.Password,
                                                         RememberMe = Input.RememberMe,
                                                         Locale = CultureInfo.CurrentCulture.Name
            };
            //var jsonContent = JsonConvert.SerializeObject(internalLogin);
            //var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            //// Send POST request to the login API
            //var response = await httpClient.PostAsync("api/auth/login", content);
           
            bool response = LoginWithCookie(internalLogin);
            Logger.LogDebug($"Form post login-end:{response}");
            if (response)
            {
                //Logger.LogInformation("User logged in.");
                var cookieOptions = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(1), Secure = true };
                HttpContext.Response.Cookies.Append("AlexTest2", "Test value1", cookieOptions);
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
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            //var result = await SignInManager.PasswordSignInAsync(Input.NickName, Input.Password, Input.RememberMe, lockoutOnFailure: true);
            //if (result.Succeeded)
            //{
            //    Logger.LogInformation("User logged in.");
            //    RedirectManager.RedirectTo(ReturnUrl);
            //}
            //else if (result.RequiresTwoFactor)
            //{
            //    RedirectManager.RedirectTo(
            //        "Account/LoginWith2fa",
            //        new() { ["returnUrl"] = ReturnUrl, ["rememberMe"] = Input.RememberMe });
            //}
            //else if (result.IsLockedOut)
            //{
            //    Logger.LogWarning("User account locked out.");
            //    RedirectManager.RedirectTo("Account/Lockout");
            //}
            //else
            //{
            //    errorMessage = "Error: Invalid login attempt.";
            //}
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
                HttpContextAccessor.HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal,
                    authProperties);
                //_cookieHandler.WriteAuthenticationCookieAsync(HttpContext,
                //    CookieAuthenticationDefaults.AuthenticationScheme,
                //    claimsPrincipal,
                //    authProperties);
                var cookieOptions = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(1), Secure = true };
                HttpContextAccessor.HttpContext.Response.Cookies.Append("AlexTest", DateTime.Now.ToLongTimeString(), cookieOptions);

                return true;

            }

            return false;
        }

        private void TestLogin()
        {
            Logger.LogDebug("***Test login");
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

        private async Task OnClickSetDemoUser()
        {
            await SetLoginCredentials("test", "Test");
        }
        const string JsTheForm = "document.forms[0]";

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        protected async Task SetLoginCredentials(string username, string password)
        {
            // Input.NickName = username;
            // Input.Password = password;
            // StateHasChanged();
            // await Task.CompletedTask;
            //await JSRuntime.InvokeVoidAsync("alert", "Warning!"); // Alert
            await JSRuntime.InvokeVoidAsync("eval", $@"{JsTheForm}.Nickname.value = '{username}'");
            await JSRuntime.InvokeVoidAsync("eval", $@"{JsTheForm}.Password.value = '{password}'");
        }
    }
}