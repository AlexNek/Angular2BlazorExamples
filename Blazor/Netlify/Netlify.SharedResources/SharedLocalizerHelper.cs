using System.Globalization;
using System.Reflection;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Netlify.SharedResources;

public static class SharedLocalizerHelper
{
    //private static readonly IOptions<LocalizationOptions> Options =
    //    Microsoft.Extensions.Options.Options.Create(new LocalizationOptions() { ResourcesPath = "Resources" });

    //private static readonly ILoggerFactory LoggerFactory =
    //    Microsoft.Extensions.Logging.LoggerFactory.Create(builder => { });

    //private static readonly IStringLocalizerFactory LocalizerFactory = new ResourceManagerStringLocalizerFactory(
    //    Options,
    //    LoggerFactory
    //);


    //public static IStringLocalizer Localizer { get; } = LocalizerFactory.Create(
    //    typeof(SharedLocalizer).Name,
    //    CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "en"
    //        ? "Netlify." + nameof(SharedLocalizer) + ".dll"
    //        : nameof(SharedLocalizer) + "." + CultureInfo.CurrentCulture.TwoLetterISOLanguageName);

    // Define supported cultures and their descriptions
    private static readonly Dictionary<string, string> SupportedCultures = new()
                                                                               {
                                                                                   { "en", "English" },
                                                                                   { "es", "Spanish" }
                                                                               };

    public static CultureInfo[] GetSupportedCultures()
    {
        var cultures = new List<CultureInfo>();
        foreach (var culture in SupportedCultures.Keys)
        {
            cultures.Add(new CultureInfo(culture));
        }

        return cultures.ToArray();
    }

    public static Dictionary<string, string> GetCulturesDescription()
    {
        return new Dictionary<string, string>(SupportedCultures);
    }

    private static object GetAssemblyWrapper(Assembly assembly)
    {
        var assemblyWrapperType = assembly.GetType("Microsoft.Extensions.Localization.AssemblyWrapper");
        var assemblyWrapperConstructor = assemblyWrapperType.GetConstructor(new[] { typeof(Assembly) });
        return assemblyWrapperConstructor.Invoke(new object[] { assembly });
    }

    //internal class AssemblyWrapper
    //{
    //    private readonly Assembly _assembly;

    //    public AssemblyWrapper(Assembly assembly)
    //    {
    //        _assembly = assembly;
    //    }

    //    public Assembly GetAssembly()
    //    {
    //        return _assembly;
    //    }
    //}
}
