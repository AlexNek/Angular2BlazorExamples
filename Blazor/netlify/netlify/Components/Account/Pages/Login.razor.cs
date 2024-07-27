using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;

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

        protected override async Task OnInitializedAsync()
        {
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