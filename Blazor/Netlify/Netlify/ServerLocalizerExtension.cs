using Netlify.Middlware;
using Netlify.SharedResources;

namespace Netlify
{
    public static class ServerLocalizerExtension
    {
        public static void AddSharedLocalization(this WebApplication app)
        {
            var supportedCultures = SharedLocalizerExtension.GetSupportedCultures();

            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions);

            app.UseMiddleware<CultureMiddleware>();

        }
    }
}
