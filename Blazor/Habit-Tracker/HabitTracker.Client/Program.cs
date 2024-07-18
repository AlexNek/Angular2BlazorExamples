using Blazorise;
using Blazorise.Icons.Material;
using Blazorise.Material;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace HabitTracker.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            AddBlazorise(builder.Services);

            await builder.Build().RunAsync();
        }

        public static void AddBlazorise(IServiceCollection services)
        {
            services
                .AddBlazorise();
            services
                .AddMaterialProviders()
                .AddMaterialIcons();
        }
    }
}