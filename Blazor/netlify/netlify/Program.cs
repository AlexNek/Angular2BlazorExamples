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
            services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();
           
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
            services.AddSession();
            
           // Configure localization services
            services.AddSharedLocalization();

            services.AddScoped<ServerState>();
            services.AddScoped<CircuitHandler, AppCircuitHandler>();
            services.AddSingleton<InMemoryUserStateService>();

            services.AddScoped<IdentityRedirectManager>();

            services.AddHttpContextAccessor();

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromMinutes(30);
                    options.Cookie.HttpOnly = true;// Prevent client-side access
                    options.Cookie.IsEssential = true; // Ensure session cookie is always stored
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensure HTTPS is used
                    options.Cookie.SameSite = SameSiteMode.Strict; // Mitigate CSRF risks
                });

            services.AddControllers();
            //.AddJsonOptions(options =>
            //    {
            //        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            //    });

            var apiServerUrl = builder.Configuration.GetSection("ApiServerUrl").Value;
            services.AddLocalization();
            //services.InitApiClient<AppConfig>("https://nestjs-example-app.fly.dev");
            services.InitApiClient<AppConfig>(apiServerUrl);

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
            

            app.UseSession(); // Make sure this is added before UseAuthentication and UseAuthorization
            //app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<TokenInterceptorMiddleware>();

            app.UseAntiforgery();// Must be after UseAuthorization!
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
