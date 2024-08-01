# Blazor Implementing Custom Authentication
## Overview
In this section, we implement simple cookie-based authorization for our Blazor .NET 8.0 project, utilizing the Auto render mode. This setup is essential for ensuring that all authentication logic is securely handled on the server side, especially as we plan to integrate an external user management API in the future.
### Key Considerations
- **Cookie-Based Authorization**: We rely on cookies to manage user sessions and authentication state. This method is robust for maintaining user login sessions across the application.
- **Auto Render Mode**: By using Auto render mode, we ensure that the rendering logic dynamically updates based on the user's authentication state. This provides a seamless user experience where the UI reflects the user's login status in real-time.
- **Server-Side Login Logic**: All authentication processes, including user login and session management, are handled on the server side. This design choice not only centralizes the security logic but also allows for easy integration with external services, such as a user management API, without exposing sensitive client-side operations.

This setup provides a secure and efficient foundation for managing user authentication and will be crucial as we expand our application to include more advanced features and integrations.

## Set Up Your Blazor Projects Variant I

In this setup, the login and logout pages are handled on the server side. The `HttpContext.SignInAsync(...)` method is invoked during the login process to issue the authentication cookie. After the user is authenticated, a full page reload is enforced on the server side to apply the cookie and update the authentication state on the client side

To navigate to a test page and force a full reload, use the following code snippet:

```csharp
NavigationManager.NavigateTo("/secure-page", forceLoad: true);
```

> **Note**: Pay attention that the authentication cookie storing and the user state will be updated only after a complete page reload.

### Server Side
Configure Authentication in Program.cs

```csharp
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(
                    options =>
                        {
                            options.LoginPath = "auth/log-in";
                            options.AccessDeniedPath = "/access-denied";
                        });

            builder.Services.AddAuthorizationCore();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.UseAuthentication();
            app.UseAuthorization();
```

#### Create a Login component:

Important part only, details see into source code
```csharp
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
            }
            else
            {
                errorMessage = "Error: Invalid login attempt.";
            }
        }
```

#### Create a Logout component

```csharp
@page "/auth/logout"
@using Microsoft.AspNetCore.Authentication
@using Microsoft.AspNetCore.Authentication.Cookies
@using Microsoft.AspNetCore.Components.Authorization
@using Netlify.Client
@inject NavigationManager Navigation


<button @onclick="LogoutHandler">Logout</button>

@code {
    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    private async Task LogoutHandler()
    {
        // Sign out the user by clearing the authentication cookie
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        Navigation.NavigateTo("/",true);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await LogoutHandler();
    }
}

```
### Client Side
#### Install Necessary Packages

To set up cookie-based authentication in your Blazor project, you'll need to install the following packages on the client side:

```bash
dotnet add package Microsoft.AspNetCore.Components.Authorization
dotnet add package Microsoft.AspNetCore.Authentication.Cookies
```

#### Add Authentication and Authorization check
```
@page "/secure-page"
@attribute [Authorize]

<AuthorizeView>
    <Authorized>
        <h3>Secure Page</h3>
        <p>This content is only visible to authenticated users.</p>
    </Authorized>
    <NotAuthorized>
        <h3>Access Denied</h3>
        <p>You are not authorized to view this content.</p>
    </NotAuthorized>
</AuthorizeView>
```

## Set Up Your Blazor Projects Variant II
In this setup, the login and logout pages must be handled on the client side only. The `HttpContext.SignInAsync(...)` method is invoked during the login process to issue the authentication cookie into controller on server side. After the user is authenticated, a full page reload is enforced on the server side by API to apply the cookie and update the authentication state on the client side

### Server Side
```csharp
            // Read base address from configuration
            var baseAddress = builder.Configuration["ApiSettings:BaseAddress"];
            builder.Services.AddHttpClient<BaseAuthenticationStateProvider>( client => { client.BaseAddress = new Uri(baseAddress); });

            builder.Services.AddControllers();
```

#### Create Authentication Endpoints on the Server
Create a new API controller in Server/Controllers/AuthController.cs:
```csharp
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel login)
    {
        if (login.Username == "test" && login.Password == "password") // Replace with real user validation
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, login.Username),
                new Claim(ClaimTypes.Role, "User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = login.RememberMe
            };

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return Ok();
        }

        return Unauthorized();
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok();
    }
}

public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}
```

### Client Side
#### Create a Login component:

```csharp
private async Task HandleLogin()
    {
        var response = await Http.PostAsJsonAsync("api/auth/login", loginModel);
        if (result.IsSuccessStatusCode)
        {
            Navigation.NavigateTo("/", true);
        }
        else
        {
            errorMessage = "Invalid login attempt.";
        }
    }
```

#### Create a Logout component

```csharp
@page "/logout"
@inject NavigationManager Navigation
@inject HttpClient Http
@inject CustomAuthStateProvider AuthStateProvider

<button @onclick="Logout">Logout</button>

@code {
    private async Task Logout()
    {
        await Http.PostAsync("api/auth/logout", null);
        AuthStateProvider.NotifyUserLogout();
        Navigation.NavigateTo("/");
    }
}

```
#### Handle Authentication State in Blazor

Create a custom authentication state provider in Client/CustomAuthStateProvider.cs:

```csharp
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;

    public CustomAuthStateProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await _httpClient.GetFromJsonAsync<ClaimsPrincipal>("api/auth/user");

        if (user == null)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        return new AuthenticationState(user);
    }

    public void NotifyUserAuthentication(ClaimsPrincipal user)
    {
        var authState = Task.FromResult(new AuthenticationState(user));
        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout()
    {
        var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        NotifyAuthenticationStateChanged(authState);
    }
}

```
Register the custom authentication state provider in Client/Program.cs:

```csharp
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();
```



