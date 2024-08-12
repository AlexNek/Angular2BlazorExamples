using Blazr.RenderState.WASM;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

using Netlify.SharedResources;

namespace Netlify.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.AddBlazrRenderStateWASMServices(); // Library for showing render state

            var services = builder.Services;
           
            services.AddAuthorizationCore();
            //services.AddCascadingAuthenticationState();
            services.AddCascadingAuthenticationState();
            services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

            // Configure localization services
            services.AddSharedLocalization();

            // need for FluentUI sometimes
            builder.Services.AddFluentUIComponents();
            //services.AddSingleton<LibraryConfiguration>();


            var host = builder.Build();

            await host.AddSharedLocalization("en-US");

            await host.RunAsync();
        }
    }
}
