namespace Netlifly.Shared.Response
{
    // Translated from TypeScript to C#

    public class UpdateUserResponse
    {
        public DataObject? Data { get; set; }

        public object? Errors { get; set; }

        public class DataObject
        {
            public User UpdateUser { get; set; }
        }
    }
}
