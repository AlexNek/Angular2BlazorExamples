using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Claims;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;

using Netlifly.Shared;
using Netlifly.Shared.Request;

using Netlify.ApiClient.Auth;

namespace Netlify.Components.Account.Components;

public partial class EditProfile
{
    private IEnumerable<IdentityError>? _identityErrors;

    private bool _isProfileButtonLoading;
    
    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; }

    //[Inject]
    //private ServerState ServerState { get; set; }

    [Inject]
    private IAuthService ApiClient { get; set; }

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    private string? Message =>
        _identityErrors is null
            ? null
            : $"Error: {string.Join(", ", _identityErrors.Select(error => error.Description))}";

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    public async Task UpdateUser(EditContext editContext)
    {
        try
        {
            User user = new User() { Id = Input.Id, Email = Input.Email };
            UpdateUserData userData = new UpdateUserData { User = user, FirstName = Input.Name };
            var authUserData = await ApiClient.UpdateUserAsync(userData);
            if (authUserData != null)
            {
                HandleUpdateUserResponse();
                //RedirectManager.RedirectTo(ReturnUrl);
            }
            else
            {
                _identityErrors = [new IdentityError { Description = "Update is not successful" }];
            }
        }
        catch (Exception ex)
        {
            _identityErrors = [new IdentityError { Description = ex.Message }];
            HandleUpdateUserError(ex);
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        //var user = ServerState.User;
        if (user.Identity is { IsAuthenticated: true })
        {
            Input.Id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Input.Email = user.FindFirst(ClaimTypes.Email)?.Value;
            Input.Name = user.Identity.Name;
        }

        //bool isRunningOnWasm = IsRunningOnWasm();
    }

    //public bool IsRunningOnWasm()
    //{
    //    return RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER"));
    //}
    /// <summary>
    /// Method invoked when the component has received parameters from its parent in
    /// the render tree, and the incoming values have been assigned to properties.
    /// </summary>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
       
    }

    private void HandleUpdateUserError(Exception error)
    {
        var networkError = UtilService.CheckNetworkError(error);
        if (networkError)
        {
            ToastService.ShowError(AlertId.NetworkError.ToStringLocalized(Localizer));
        }
        else
        {
            //var registerErrors = error.GraphQLErrors;
            //if (registerErrors.Any())
            {
                ToastService.ShowError(AlertId.UpdateUserError.ToStringLocalized(Localizer));
            }
        }

        _isProfileButtonLoading = networkError;
        //StateHasChanged();
    }

    private void HandleUpdateUserResponse()
    {
        ToastService.ShowSuccess(AlertId.UserSaved.ToStringLocalized(Localizer));
        _isProfileButtonLoading = false;
        StateHasChanged();
    }

    private sealed class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email
        {
            get; 
            set;
        } = string.Empty;

        //Hidden userId
        public string? Id { get; set; }

        [Required]
        [Display(Name = "NickName")]
        [MinLength(2)]
        public string Name { get; set; } = string.Empty;
    }
}

internal class UtilService
{
    public static bool CheckNetworkError(Exception ex)
    {
        return ex is HttpRequestException || ex is SocketException || ex is WebException;
    }
}

//public class EditProfileComponent : ComponentBase, IDisposable
//{
//    [Parameter]
//    public User User { get; set; }

//    private Subject<bool> destroy = new Subject<bool>();

//    private bool isProfileButtonLoading;
//    private EditForm profileForm;
//    private FormControl<string> firstname;
//    private FormControl<string> email;

//    public EditProfileComponent()
//    {
//        isProfileButtonLoading = false;
//    }

//    protected override void OnInitialized()
//    {
//        firstname = new FormControl<string>(User?.Firstname ?? string.Empty,
//            new[] { Validators.Required(), Validators.MinLength(2) });
//        email = new FormControl<string>(new FormControlValue<string> { Value = User?.Email ?? string.Empty, Disabled = true });

//        profileForm = new EditForm
//        {
//            Controls = new
//            {
//                Firstname = firstname,
//                Email = email
//            }
//        };
//    }

//    private void SendForm()
//    {
//        if (profileForm.IsValid && User != null)
//        {
//            isProfileButtonLoading = true;

//            var formValue = profileForm.GetRawValue();
//            authService.UpdateUser(new User
//            {
//                Id = User.Id,
//                Firstname = formValue.Firstname,
//                Email = User.Email
//            })
//            .TakeUntil(destroy)
//            .Subscribe(
//                _ => HandleUpdateUserResponse(),
//                error => HandleUpdateUserError(error)
//            );
//        }
//    }

//    private void HandleUpdateUserResponse()
//    {
//        alertService.Create(AlertId.UserSaved);
//        isProfileButtonLoading = false;
//        StateHasChanged();
//    }

//    private void HandleUpdateUserError(ApolloError error)
//    {
//        var networkError = utilService.CheckNetworkError(error);
//        if (!networkError)
//        {
//            var registerErrors = error.GraphQLErrors;
//            if (registerErrors.Any())
//            {
//                alertService.Create(AlertId.UpdateUserError);
//            }
//        }
//        isProfileButtonLoading = networkError;
//        StateHasChanged();
//    }

//    public void Dispose()
//    {
//        destroy.OnNext(true);
//        destroy.Dispose();
//    }
//}
