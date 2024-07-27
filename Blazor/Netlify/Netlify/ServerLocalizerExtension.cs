using Netlify.Middlware;
using Netlify.SharedResources;

namespace Netlify
{
    public static class ServerLocalizerExtension
    {
        public static void AddSharedLocalization(this WebApplication app)
        {
            var supportedCultures = SharedLocalizerHelper.GetSupportedCultures();
            var cultureNames = supportedCultures.Select(c => c.Name).ToArray();
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(cultureNames[0])
                .AddSupportedCultures(cultureNames)
                .AddSupportedUICultures(cultureNames);

            app.UseRequestLocalization(localizationOptions);

            app.UseMiddleware<CultureMiddleware>();
            // Ensure localization is set up
            var localizer = app.Services.GetService<SharedLocalizer>();
            StaticLocalizer.SetLocalizer(localizer);

        }
    }
}
