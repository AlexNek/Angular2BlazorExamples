using System.Globalization;

namespace Netlify.SharedResources;

public static class SharedLocalizerHelper
{
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
}
