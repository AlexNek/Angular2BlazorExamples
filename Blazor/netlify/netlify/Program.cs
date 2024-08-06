using Blazored.LocalStorage;

using Blazr.RenderState.Server;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Identity;
using Microsoft.FluentUI.AspNetCore.Components;

using Netlify.ApiClient;
using Netlify.Client;
using Netlify.Components;
using Netlify.Components.Account;
using Netlify.Middlware;
using Netlify.SharedResources;

using Serilog;

namespace Netlify
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.AddBlazrRenderStateServerServices();

            // Add services to the container.
            var services = builder.Services;
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.txt",
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();

            services.AddSerilog();

            //services.AddLogging();
           
            services.AddCascadingAuthenticationState();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                    {
                        // 2 lines for test
                        //options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Use 'Always' in production with HTTPS
                        //options.Cookie.SameSite = SameSiteMode.Lax; // Adjust based on your needs (or SameSiteMode.Strict)

                        options.LoginPath = "/auth/log-in"; // route to login
                        options.LogoutPath = "/auth/logout"; // route to logout
                        options.AccessDeniedPath = "/access-denied";
                    });

            services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();
            services.AddFluentUIComponents();
            
           // Configure localization services
            services.AddSharedLocalization();

            services.AddScoped<ServerState>();
            services.AddScoped<CircuitHandler, AppCircuitHandler>();
            services.AddSingleton<InMemoryUserStateService>();

            services.AddScoped<IdentityRedirectManager>();

            services.AddHttpContextAccessor();

            services.AddControllers();
                //.AddJsonOptions(options =>
                //    {
                //        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                //    });

            services.AddLocalization();
            services.InitApiClient<AppConfig>("https://nestjs-example-app.fly.dev");

            // Register HttpClient as Singleton (recommended)
            //services.AddHttpClient<CustomAuthStateProvider>(client =>
            //    {
            //        client.BaseAddress = new Uri("https://localhost:7254"); // Set your backend URL here
            //    });

            services.AddAuthorizationCore();

            services.AddBlazoredLocalStorage();

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

            app.AddSharedLocalization();

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            //app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

            // Add additional endpoints required by the Identity /Account Razor components.
            //app.MapAdditionalIdentityEndpoints();

            app.Run();
        }
        
    }
}
