using Netlifly.Shared;

namespace Netlify;

public class AppConfig : IAppConfig
{
    public int AlertMilliseconds { get;  } = 3000;

    public IAppConfig.LocalBreakpoints Breakpoints { get; set; } =
        new IAppConfig.LocalBreakpoints()
            {
                Xs = 0,
                Sm = 576,
                Md = 768,
                Xl = 1200,
                Xxl = 1400
            };

    public string BypassAuthorization { get; set; } = "bypassAuthorization";

    public IAppConfig.LocalCustomQueryParams CustomQueryParams { get; set; } =
        new IAppConfig.LocalCustomQueryParams() { AlertId = "alertId", Origin = "origin" };

    public string DefaultLang { get; set; } = "en";

    public IAppConfig.LocalEndpoints Endpoints { get; set; } = new IAppConfig.LocalEndpoints() { Graphql = "graphql" };

    public IAppConfig.LocalLanguages Languages { get; set; } = new IAppConfig.LocalLanguages() { En = "en", Es = "es" };
}
