using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

using Netlifly.Shared.Request;

using Netlify.ApiClient.Auth;
using Netlify.Attributes;

namespace Netlify.Components.Account.Pages;

public partial class Register
{
    private IEnumerable<IdentityError>? identityErrors;

    [Inject]
    private IAuthService apiClient { get; set; }

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    private string? Message =>
        identityErrors is null
            ? null
            : $"Error: {string.Join(", ", identityErrors.Select(error => error.Description))}";

    public async Task RegisterUser(EditContext editContext)
    {
        RegisterPayload registerData = new RegisterPayload(){FirstName = Input.Name, Email = Input.Email, Password = Input.Password};
        try
        {
            var authUserData = await apiClient.SignupAsync(registerData);
            if (authUserData != null)
            {
                RedirectManager.RedirectTo(ReturnUrl);
            }
            else
            {
                identityErrors = [new IdentityError() { Description = "Registration is not successful" }];
            }
        }
        catch (Exception ex)
        {
            identityErrors = [new IdentityError() { Description = ex.Message }];
        }

        //var user = CreateUser();

        //await UserStore.SetUserNameAsync(user, Input.Name, CancellationToken.None);
        //var emailStore = GetEmailStore();
        //await emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
        //var result = await UserManager.CreateAsync(user, Input.Password);

        //if (!result.Succeeded)
        //{
        //    identityErrors = result.Errors;
        //    return;
        //}

        //Logger.LogInformation("User created a new account with password.");

        //var userId = await UserManager.GetUserIdAsync(user);
        //var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
        //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        //var callbackUrl = NavigationManager.GetUriWithQueryParameters(
        //    NavigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
        //    new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code, ["returnUrl"] = ReturnUrl });

        //await EmailSender.SendConfirmationLinkAsync(user, Input.Email, HtmlEncoder.Default.Encode(callbackUrl));

        //if (UserManager.Options.SignIn.RequireConfirmedAccount)
        //{
        //    RedirectManager.RedirectTo(
        //        "Account/RegisterConfirmation",
        //        new() { ["name"] = Input.Name, ["email"] = Input.Email, ["returnUrl"] = ReturnUrl });
        //}

        //await SignInManager.SignInAsync(user, isPersistent: false);
       
    }

    //private ApplicationUser CreateUser()
    //{
    //    try
    //    {
    //        return Activator.CreateInstance<ApplicationUser>();
    //    }
    //    catch
    //    {
    //        throw new InvalidOperationException(
    //            $"Can't create an instance of '{nameof(ApplicationUser)}'. " +
    //            $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor.");
    //    }
    //}

    //private IUserEmailStore<ApplicationUser> GetEmailStore()
    //{
    //    if (!UserManager.SupportsUserEmail)
    //    {
    //        throw new NotSupportedException("The default UI requires a user store with email support.");
    //    }

    //    return (IUserEmailStore<ApplicationUser>)UserStore;
    //}

    private sealed class InputModel
    {
        [Required]
        [Display(Name = "NickName")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = "";

        [Required]
        [PasswordLengthDefaultLoc]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [CompareLoc("Password", "The password and confirmation password do not match.") ]
        public string ConfirmPassword { get; set; } = "";

        [MustBeTrueLoc("You must accept the terms and conditions")]
        public bool Accept { get; set; }
    }
}