using Netlifly.Shared.Request;

namespace Netlifly.Shared.Response
{
    // Translation from TypeScript to C#

    // Define LogInResponse interface
    public class LogInResponse
    {
        // Data field containing login object with accessToken, refreshToken, and user properties
        public DataObject? Data { get; set; }

        // Optional errors field of unknown type
        public object? Errors { get; set; }

        public class DataObject
        {
            public AuthUserData? Login { get; set; }
        }

    }
}
