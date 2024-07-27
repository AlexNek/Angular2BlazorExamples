namespace Netlify.SharedResources;

public static class StaticLocalizer
{
    private static SharedLocalizer _localizer;

    public static void SetLocalizer(SharedLocalizer localizer)
    {
        _localizer = localizer;
    }

    public static string GetString(string key)
    {
        return _localizer?[key] ?? key;
    }
}
