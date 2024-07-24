using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Netlify.SharedResources
{
    public static class SharedLocalizerExtension
    {
        public static void AddSharedLocalization(this IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddTransient<SharedLocalizer>();
        }

        public static string[] GetSupportedCultures()
        {
            return new[] { "en-US", "es-MX" };
        }
        
    }
}
