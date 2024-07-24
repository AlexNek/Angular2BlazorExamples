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

            var services = builder.Services;
           
            services.AddAuthorizationCore();
            //services.AddCascadingAuthenticationState();

            // Configure localization services
            services.AddSharedLocalization();

            // need for FluentUI sometimes
            services.AddSingleton<LibraryConfiguration>();


            var host = builder.Build();

            await host.AddSharedLocalization("en-US");

            await host.RunAsync();
        }
    }
}
