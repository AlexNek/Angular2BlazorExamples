using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;

using Netlifly.Shared;

namespace Netlify.Client;

public class CustomAuthStateProvider2 : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;

    private readonly NavigationManager _navigationManager;

    private readonly IServiceProvider _serviceProvider;

    public CustomAuthStateProvider2(
        IHttpClientFactory httpClientFactory,
        NavigationManager navigationManager,
        IServiceProvider serviceProvider)
    {
        _httpClient = httpClientFactory.CreateClient(nameof(CustomAuthStateProvider2));
        _navigationManager = navigationManager;
        _serviceProvider = serviceProvider;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        ClaimsPrincipal user = new ClaimsPrincipal(new ClaimsIdentity());

        // Check if we are on Blazor Server or WebAssembly
        if (_serviceProvider.GetService<IHttpContextAccessor>() is IHttpContextAccessor httpContextAccessor)
        {
            // Blazor Server - Directly use HttpContext
            var context = httpContextAccessor.HttpContext;
            if (context.User.Identity is { IsAuthenticated: true })
            {
                user = context.User;
            }
            else
            {
                user = new ClaimsPrincipal(new ClaimsIdentity());
            }
        }
        else
        {
            // Blazor WebAssembly - Check via HttpClient
            InternalLoginResponse? userInfo = null;

            try
            {
                //userInfo = await _httpClient.GetFromJsonAsync<InternalLoginResponse>("api/auth/user");
                var response = await _httpClient.GetAsync("api/auth/user");

                // Ensure the response is successful
                response.EnsureSuccessStatusCode();

                // Read the response content as a string for debugging
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Content: {responseContent}");

                // Deserialize the response
                userInfo = JsonSerializer.Deserialize<InternalLoginResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        //ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
                    });
                var claims = userInfo.Claims.Select(c => new Claim(c.Type, c.Value)).ToList();
                var identity = new ClaimsIdentity(claims, "serverauth");
                user = new ClaimsPrincipal(identity);
            }
            catch (Exception ex)
            {
                //ignore 401
            }
            
        }

        return new AuthenticationState(user);
    }
    //public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    //{
    //    InternalLoginResponse? userInfo = null;

    //    try
    //    {
    //        //userInfo = await _httpClient.GetFromJsonAsync<InternalLoginResponse>("api/auth/user");
    //        var response = await _httpClient.GetAsync("api/auth/user");

    //        // Ensure the response is successful
    //        response.EnsureSuccessStatusCode();

    //        // Read the response content as a string for debugging
    //        var responseContent = await response.Content.ReadAsStringAsync();
    //        Console.WriteLine($"Response Content: {responseContent}");

    //        // Deserialize the response
    //        userInfo = JsonSerializer.Deserialize<InternalLoginResponse>(responseContent, new JsonSerializerOptions
    //            {
    //                PropertyNameCaseInsensitive = true,
    //                //ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
    //            });
    //    }
    //    catch (Exception ex)
    //    {
    //        //ignore 401
    //    }

    //    if (userInfo == null || !userInfo.IsAuthenticated)
    //    {
    //        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    //    }

    //    var claims = userInfo.Claims.Select(c => new Claim(c.Type, c.Value)).ToList();
    //    var identity = new ClaimsIdentity(claims, "serverauth");
    //    var user = new ClaimsPrincipal(identity);

    //    return new AuthenticationState(user);
    //}

    public void NotifyUserLogin(string email)
    {
        var identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.Name, email) },
            CookieAuthenticationDefaults.AuthenticationScheme);

        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void NotifyUserLogout()
    {
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
    }
}
