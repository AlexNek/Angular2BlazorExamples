namespace Netlifly.Shared
{
    public class AppConfig
    {
        public int AlertMilliseconds { get; set; }

        public LocalBreakpoints Breakpoints { get; set; }

        public string BypassAuthorization { get; set; }

        public LocalCustomQueryParams CustomQueryParams { get; set; }

        public string DefaultLang { get; set; }

        public LocalEndpoints Endpoints { get; set; }

        public LocalLanguages Languages { get; set; }

        public class LocalBreakpoints
        {
            public int Lg { get; set; }

            public int Md { get; set; }

            public int Sm { get; set; }

            public int Xl { get; set; }

            public int Xs { get; set; }

            public int Xxl { get; set; }
        }

        public class LocalCustomQueryParams
        {
            public string AlertId { get; set; }

            public string Origin { get; set; }
        }

        public class LocalEndpoints
        {
            public string Graphql { get; set; }
        }

        public class LocalLanguages
        {
            public string En { get; set; }

            public string Es { get; set; }
        }
    }
}
