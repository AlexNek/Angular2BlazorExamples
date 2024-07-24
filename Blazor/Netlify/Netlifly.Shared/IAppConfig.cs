namespace Netlifly.Shared;

public interface IAppConfig
{
     int AlertMilliseconds { get;  }

     LocalBreakpoints Breakpoints { get;  }

    string BypassAuthorization { get;  }

     LocalCustomQueryParams CustomQueryParams { get;  }

    string DefaultLang { get;  }

    LocalEndpoints Endpoints { get; }

    LocalLanguages Languages { get; }

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