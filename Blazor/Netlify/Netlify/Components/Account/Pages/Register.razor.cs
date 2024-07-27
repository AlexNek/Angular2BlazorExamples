using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Netlify.SharedResources;

namespace Netlify.Components.Account.Pages;

public partial class Register
{
    private IEnumerable<IdentityError>? identityErrors;

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
        RedirectManager.RedirectTo(ReturnUrl);
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
        [StringLength(
            100,
            ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [CompareLoc("Password", "The password and confirmation password do not match.") ]
        public string ConfirmPassword { get; set; } = "";

        [MustBeTrue("You must accept the terms and conditions")]
        public bool Accept { get; set; }
    }
    
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CompareLoc : CompareAttribute
    {
        private readonly string _message;

        /// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DataAnnotations.CompareAttribute" /> class.</summary>
        /// <param name="password"></param>
        /// <param name="otherProperty">The property to compare with the current property.</param>
        public CompareLoc(string otherProperty, string message)
            : base(otherProperty)
        {
            _message = message;
        }

        /// <summary>Applies formatting to an error message, based on the data field where the error occurred.</summary>
        /// <param name="name">The name of the field that caused the validation failure.</param>
        /// <returns>The formatted error message.</returns>
        public override string FormatErrorMessage(string name)
        {
            return StaticLocalizer.GetString(_message);
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MustBeTrueAttribute : ValidationAttribute
    {
        private readonly string _message;

        public MustBeTrueAttribute(string message)
        {
            _message = message;
        }
        public override bool IsValid(object? value)
        {
            // Check if value is a boolean and equals true
            return value is true;
        }

        public override string FormatErrorMessage(string name)
        {
            // Use the static localizer to get the localized error message
            return StaticLocalizer.GetString(_message); ;
        }
    }
}
