using System.Security.Claims;
using System.Text.Json;

using Microsoft.AspNetCore.Components.Authorization;

using Netlifly.Shared;

namespace Netlify.Client;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;

    //public CustomAuthStateProvider(HttpClient httpClient)
    //{
    //    _httpClient = httpClient;
    //}

    public CustomAuthStateProvider(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(nameof(CustomAuthStateProvider));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
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
        }
        catch (Exception ex)
        {
            //ignore 401
        }

        if (userInfo == null || !userInfo.IsAuthenticated)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var claims = userInfo.Claims.Select(c => new Claim(c.Type, c.Value)).ToList();
        var identity = new ClaimsIdentity(claims, "serverauth");
        var user = new ClaimsPrincipal(identity);

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
