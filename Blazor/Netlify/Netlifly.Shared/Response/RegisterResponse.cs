using Netlifly.Shared.Request;

namespace Netlifly.Shared.Response
{
    public class RegisterResponse
    {
        public DataObject? Data { get; set; }

        public object? Errors { get; set; }

        public class DataObject
        {
            public AuthUserData Signup { get; set; }
        }
    }
}
