using Netlifly.Shared.Request;

using Newtonsoft.Json;

namespace Netlifly.Shared.Response
{
    // Define LogInResponse interface
    public class LogInResponse
    {
        [JsonProperty(PropertyName = "login")]
        public AuthUserData? Login { get; set; }

    }
}
