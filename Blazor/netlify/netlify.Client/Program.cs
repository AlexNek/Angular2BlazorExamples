using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Globalization;

using Microsoft.FluentUI.AspNetCore.Components;

namespace Netlify.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            //await builder.Build().RunAsync();
            var services = builder.Services;
            services.AddLocalization();
            services.AddAuthorizationCore();
            //services.AddCascadingAuthenticationState();


            // need for FluentUI sometimes
            services.AddSingleton<LibraryConfiguration>();


            var host = builder.Build();

            const string defaultCulture = "en-US";

            var js = host.Services.GetRequiredService<IJSRuntime>();
            var result = await js.InvokeAsync<string>("blazorCulture.get");
            var culture = CultureInfo.GetCultureInfo(result ?? defaultCulture);

            if (result == null)
            {
                await js.InvokeVoidAsync("blazorCulture.set", defaultCulture);
            }

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            await host.RunAsync();
        }
    }
}
