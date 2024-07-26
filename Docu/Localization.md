# Blazor localization steps

## Overview

Localization is an essential aspect of modern web applications, enabling them to support multiple languages and regions. In this guide, we will use a complex example where both the client and server work together. We will follow the [Microsoft Recommendation](https://learn.microsoft.com/en-us/aspnet/core/blazor/globalization-localization?view=aspnetcore-8.0).

## Create a Shared Project for Resources

Create a new class library project, e.g., `SharedResources`, to hold your resource files. This project will be referenced by both the client and server projects.

![image](pics/localization-prjs.png)

1. The Server project must reference the `SharedResources` project (3).
2. The Client project must reference the `SharedResources` project (3).

Ensure that the `Microsoft.Extensions.Localization` package is included in all projects.

## Add Resource Files

Add your resource files to the `Resources` folder in the `SharedResources` project.
The default resource file can be `SharedLocalizer.resx` (for the default culture), and you can add culture-specific variants such as `SharedLocalizer.es-MX.resx`. Add any example for translation to resources.

![image](pics/localization-resx.png)

Create the `SharedLocalizer` class (A) in the `SharedResources` project. This class will be used to access the localized strings. It is important that the resource names B, C have the same name as A.

```csharp
using Microsoft.Extensions.Localization;

namespace Netlify.SharedResources
{
    public class SharedLocalizer
    {
        private readonly IStringLocalizer<SharedLocalizer> _localizer;

        public SharedLocalizer(IStringLocalizer<SharedLocalizer> localizer)
        {
            _localizer = localizer;
        }

        public string this[string key] => _localizer[key];
    }
}
```


## Adding an Extension Method for Localization

To streamline the process of adding localization services across your application, create the `SharedLocalizerExtension` class within the `SharedResources` project. This class provides an extension method to easily incorporate localization services and a method to retrieve supported cultures.

### SharedLocalizerExtension Class

The `SharedLocalizerExtension` class leverages the `IServiceCollection` interface to add localization services to the service collection. This class centralizes the localization setup, making it reusable and maintainable.

Here’s the implementation of the `SharedLocalizerExtension` class:

```csharp
using Microsoft.Extensions.DependencyInjection;

namespace Netlify.SharedResources
{
    public static class SharedLocalizerExtension
    {
        /// <summary>
        /// Adds shared localization services to the specified IServiceCollection.
        /// </summary>
        /// <param name="services">The IServiceCollection to add the services to.</param>
        public static void AddSharedLocalization(this IServiceCollection services)
        {
            // Configure localization options
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            
            // Register the SharedLocalizer service with a transient lifetime
            services.AddTransient<SharedLocalizer>();
        }
    }
}
```

### Key Points:
- **AddLocalization**: Configures localization options, setting the path for resource files to "Resources".
- **AddTransient**: Registers the `SharedLocalizer` service with a transient lifetime, meaning a new instance is created each time it is requested.

### Usage:
To use this extension method, simply call `AddSharedLocalization` on your `IServiceCollection` instance in the `Startup` class or wherever you configure your services:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSharedLocalization();
    // Other service configurations
}
```


## Adding Language Description

To define and retrieve supported cultures and their descriptions, create the `SharedLocalizerHelper` class within the `SharedResources` project. This class provides methods to get an array of supported `CultureInfo` objects and a dictionary containing culture codes and their respective descriptions. This ensures that all language descriptions are centralized in a single project.

### SharedLocalizerHelper Class

The `SharedLocalizerHelper` class manages a dictionary of supported cultures and their descriptions. It provides methods to retrieve supported cultures as well as their descriptions.

Here’s the implementation of the `SharedLocalizerHelper` class:

```csharp
using System.Globalization;

namespace Netlify.SharedResources;

public static class SharedLocalizerHelper
{
    // Define supported cultures and their descriptions
    private static readonly Dictionary<string, string> SupportedCultures = new()
    {
        { "en-US", "English (United States)" },
        { "es-MX", "Spanish (Mexico)" }
    };

    /// <summary>
    /// Gets an array of supported CultureInfo objects.
    /// </summary>
    /// <returns>An array of supported CultureInfo objects.</returns>
    public static CultureInfo[] GetSupportedCultures()
    {
        var cultures = new List<CultureInfo>();
        foreach (var culture in SupportedCultures.Keys)
        {
            cultures.Add(new CultureInfo(culture));
        }
        return cultures.ToArray();
    }

    /// <summary>
    /// Gets a dictionary of supported cultures and their descriptions.
    /// </summary>
    /// <returns>A dictionary of supported cultures and their descriptions.</returns>
    public static Dictionary<string, string> GetCulturesDescription()
    {
        return new Dictionary<string, string>(SupportedCultures);
    }
}
```

### Usage:
To use the methods provided by the `SharedLocalizerHelper` class, you can call them from anywhere in your application as needed:

```csharp
var supportedCultures = SharedLocalizerHelper.GetSupportedCultures();
var culturesDescription = SharedLocalizerHelper.GetCulturesDescription();
```

## Configure the Server Project

### Add a Reference to the Shared Resources Project

In the Blazor Server project, add a reference to the `SharedResources` project.

### Create Middleware

To set the culture based on user preferences or request, create a `CultureMiddleware` class. This middleware will read the culture information from a cookie and set the current thread's culture accordingly.

Here’s the implementation of the `CultureMiddleware` class:

```csharp
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace Netlify.Middleware
{
    public class CultureMiddleware
    {
        private readonly RequestDelegate _next;

        public CultureMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var requestCookie = httpContext?.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];
            if (requestCookie != null)
            {
                var cultureResult = CookieRequestCultureProvider.ParseCookieValue(requestCookie);
                if (cultureResult != null && cultureResult.Cultures.Any())
                {
                    var currentCulture = cultureResult.Cultures[0];
                    CultureInfo cultureInfo = new CultureInfo(currentCulture.ToString());
                    CultureInfo.CurrentCulture = cultureInfo;
                    CultureInfo.CurrentUICulture = cultureInfo;
                }
            }
            await _next(httpContext);
        }
    }
}
```

### Usage:
To use this middleware, ensure that it is added to the request pipeline in the `Configure` method of your `Startup` class or `Program.cs` over `AddSharedLocalization()` call.

By integrating this middleware, you ensure that the application correctly sets the culture based on user preferences stored in cookies, enhancing the localization experience in your Blazor Server application.

### Add Extension Class

Create an extension class to configure localization in your Blazor Server project. This class will set up the supported cultures and apply the necessary middleware for handling culture-specific requests.

Here’s the implementation of the `ServerLocalizerExtension` class:

```csharp
using Netlify.Middleware;
using Netlify.SharedResources;

namespace Netlify
{
    public static class ServerLocalizerExtension
    {
        /// <summary>
        /// Adds shared localization services and middleware to the specified WebApplication.
        /// </summary>
        /// <param name="app">The WebApplication to configure.</param>
        public static void AddSharedLocalization(this WebApplication app)
        {
            var supportedCultures = SharedLocalizerHelper.GetSupportedCultures();

            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0].Name)
                .AddSupportedCultures(supportedCultures.Select(c => c.Name).ToArray())
                .AddSupportedUICultures(supportedCultures.Select(c => c.Name).ToArray());

            app.UseRequestLocalization(localizationOptions);

            app.UseMiddleware<CultureMiddleware>();
        }
    }
}
```


### Configure Localization Services

In the `Program.cs` or `Startup.cs` of the Blazor Server project, configure the localization services to use the resources from the `SharedResources` project.

#### Startup.cs (Blazor Server)

Add the necessary configuration in the `ConfigureServices` and `Configure` methods to set up localization services and middleware.

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Add shared localization services
        services.AddSharedLocalization();
        
        // Other service configurations
        ...
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            // Development-specific configurations
            ...
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        // Add shared localization middleware
        app.AddSharedLocalization();

        // Other middleware configurations
        ...
    }
}
```

By following this configuration, you ensure that the Blazor Server project uses the localization resources from the `SharedResources` project and applies the necessary middleware to handle culture-specific requests.

### Create JavaScript Interop (optional)

If you want to switch languages for both client and server projects using only cookies, you can achieve this by handling the culture settings exclusively with cookies or use local browser storage, as recommended by Microsoft.

Add the following JavaScript to your project to handle getting and setting the culture in the browser's local storage. Include this script in your `App.razor` (after blazor script):

```javascript
<script src="_framework/blazor.web.js"></script>
<script>
    window.blazorCulture = {
        get: () => localStorage['BlazorCulture'],
        set: (value) => localStorage['BlazorCulture'] = value
    };
</script>
```
Variant with cookie:
```javascript
<script>
    window.blazorCulture = {
        get: () => {
            let name = ".AspNetCore.Culture="; // Default cookie name used by CookieRequestCultureProvider
            let decodedCookie = decodeURIComponent(document.cookie);
            let ca = decodedCookie.split(';');
            for(let i = 0; i < ca.length; i++) {
                let c = ca[i];
                while (c.charAt(0) == ' ') {
                    c = c.substring(1);
                }
                if (c.indexOf(name) == 0) {
                    return c.substring(name.length, c.length);
                }
            }
            return "";
        },
        set: (value) => {
            const d = new Date();
            d.setTime(d.getTime() + (365*24*60*60*1000)); // 1 year expiry
            let expires = "expires="+ d.toUTCString();
            document.cookie = ".AspNetCore.Culture=" + value + ";" + expires + ";path=/";
        }
    };
</script>
```
This approach ensures that the culture information is correctly stored in a cookie and can be accessed both server-side and client-side as needed over additional shared service (not described there).

### Create a Localization Controller (optional)

To enable users to switch between supported cultures, create a `CultureController` that handles the setting of the culture through cookies. This controller will allow users to change the application's culture and redirect them to the specified URI.

Here’s the implementation of the `CultureController` class:

```csharp
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Netlify.Controllers
{
    [Route("[controller]/[action]")]
    public class CultureController : Controller
    {
        /// <summary>
        /// Sets the specified culture and redirects to the provided URI.
        /// </summary>
        /// <param name="culture">The culture to set.</param>
        /// <param name="redirectUri">The URI to redirect to after setting the culture.</param>
        /// <returns>A redirection result to the specified URI.</returns>
        public IActionResult Set(string culture, string redirectUri)
        {
            if (culture != null)
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1) // Set a long expiration time
                };
                string cookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture, culture));
                HttpContext.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    cookieValue,
                    cookieOptions);
            }

            return LocalRedirect(redirectUri);
        }
    }
}
```

### Usage:
To change the culture, you can call the `Set` action on the `CultureController` with the desired culture and the redirect URI. For example:

```html
<a href="/Culture/Set?culture=en-US&redirectUri=/">English</a>
<a href="/Culture/Set?culture=es-MX&redirectUri=/">Español</a>
```

By implementing this controller, you provide users with the ability to switch between different supported cultures, enhancing the localization experience in your Blazor Server application. As we are using a controller, we can switch languages from either the client or the server project.





## Configure the Client Project

### Add a Reference to the Shared Project
In the Blazor WebAssembly client project, add a reference to the `SharedResources` project.

### Add Extension Class

Create an extension class to handle language switching, incorporating JavaScript interop to store and retrieve the user's preferred culture.

Here’s the implementation of the `ClientLocalizerExtension` class:

```csharp
using System.Globalization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

namespace Netlify.Client
{
    public static class ClientLocalizerExtension
    {
        /// <summary>
        /// Adds shared localization services to the WebAssembly host.
        /// </summary>
        /// <param name="host">The WebAssembly host.</param>
        /// <param name="defaultCulture">The default culture to set if no culture is found.</param>
        public static async Task AddSharedLocalization(this WebAssemblyHost host, string defaultCulture)
        {
            var js = host.Services.GetRequiredService<IJSRuntime>();
            var result = await js.InvokeAsync<string>("blazorCulture.get");
            var culture = CultureInfo.GetCultureInfo(result ?? defaultCulture);

            if (result == null)
            {
                await js.InvokeVoidAsync("blazorCulture.set", defaultCulture);
            }

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}
```
By integrating this extension method and JavaScript interop, you ensure that the Blazor WebAssembly client project can handle culture settings, allowing for language switching based on user preferences.

### Configure Localization Services

In `Program.cs` of the Blazor WebAssembly project, configure the localization services to use the resources from the `SharedResources` project.

#### Program.cs (Blazor WebAssembly)

Add the necessary configuration to set up localization services.

```csharp
var builder = WebAssemblyHostBuilder.CreateDefault(args);

var services = builder.Services;
// Add other services
...

// Configure localization services
services.AddSharedLocalization();

var host = builder.Build();

// Configure localization settings
await host.AddSharedLocalization("en-US");

await host.RunAsync();
```

By following this configuration, you ensure that the Blazor WebAssembly project uses the localization resources from the `SharedResources` project and applies the necessary settings for handling culture-specific preferences.

### Language Selection Component

Create a Blazor component for language selection that allows users to switch between supported cultures. This component will use JavaScript interop to save the selected culture in the browser's local storage (*for our example*) and redirect to apply the new culture.

Here’s the implementation of the language selection component:

```razor
@using System.Globalization
@using Netlify.SharedResources
@inject IJSRuntime JS
@inject NavigationManager Navigation

<div style="display: flex; justify-content: center; align-items: center;">
    @Label <FluentIcon Value="@(new Icons.Regular.Size24.GlobeLocation())" Color="@IconColor" Style="margin-top: 0px"/>
    <select id="lang-select" @bind="@selectedCulture" @bind:after="ApplySelectedCultureAsync">
        @foreach (var culture in supportedCultures)
        {
            <option value="@culture">@cultureDict[culture.Name]</option>
        }
    </select>
</div>

@code {
    [Parameter]
    public string? Label { get; set; }
    
    [Parameter]
    public Color? IconColor { get; set; }

    private Dictionary<string, string> cultureDict = SharedLocalizerHelper.GetCulturesDescription();
    private CultureInfo[] supportedCultures = SharedLocalizerHelper.GetSupportedCultures();
    private CultureInfo? selectedCulture;

    protected override void OnInitialized()
    {
        selectedCulture = CultureInfo.CurrentCulture;
    }

    private async Task ApplySelectedCultureAsync()
    {
        if (CultureInfo.CurrentCulture != selectedCulture)
        {
            await JS.InvokeVoidAsync("blazorCulture.set", selectedCulture!.Name);

            var uri = new Uri(Navigation.Uri).GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
            var cultureEscaped = Uri.EscapeDataString(selectedCulture.Name);
            var uriEscaped = Uri.EscapeDataString(uri);

            Navigation.NavigateTo($"Culture/Set?culture={cultureEscaped}&redirectUri={uriEscaped}", forceLoad: true);
        }
    }
}
```

## Using the `CultureSelector` Component

To integrate the `CultureSelector` component into your Blazor application, you have at least two options for calling it:

1. With Text and a White-Colored Icon
2. Without Text

Ensure you include `@rendermode="InteractiveAuto"` when using server-side rendering (SSR) as the default mode.

### Call with Text and White-Colored Icon

```razor
<CultureSelector Label="Select your locale:" IconColor="Color.Fill" @rendermode="InteractiveAuto" />
```

### Simple Call Without Text

```razor
<CultureSelector @rendermode="InteractiveAuto" />
```
#### Example Usage

Here is how you can integrate the `CultureSelector` component into a Blazor page or layout:

```razor
@page "/"

<h3>Welcome to the Localization Example</h3>

<!-- Call with text and white colored icon -->
<CultureSelector Label="Select your locale:" IconColor="Color.Fill" @rendermode="InteractiveAuto" />

<!-- Simple call without text -->
<CultureSelector @rendermode="InteractiveAuto" />
```

### Visual Representation

Below is an example of how the language selector might look:

![Language Selector Example](pics/lang-selector.png)

By using these examples, you can provide users with an intuitive way to switch between different supported cultures in your Blazor application.

## Use the Localizer in Components

Inject the `SharedLocalizer` into your Blazor components and use it to access localized strings. This allows for a consistent and maintainable approach to supporting multiple languages and cultures across both client and server projects.

### ExampleComponent.razor (Client Project)

In the Blazor WebAssembly client project, inject the `SharedLocalizer` to use localized strings.

```razor
@page "/example"
@inject SharedLocalizer Localizer

<h1>@Localizer["HelloWorld"]</h1>
```

### ExampleComponent.razor (Server Project)

In the Blazor Server project, inject the `SharedLocalizer` to use localized strings.

```razor
@page "/example"
@inject SharedLocalizer Localizer

<h1>@Localizer["HelloWorld"]</h1>
```

### Summary

This guide covers the essential steps for setting up localization in a Blazor project with both client and server components, ensuring a consistent and maintainable approach to supporting multiple languages and cultures.

By following these approaches, you ensure a centralized and maintainable way to manage supported cultures and their descriptions across your application.
