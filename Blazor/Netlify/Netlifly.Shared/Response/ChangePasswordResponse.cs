namespace Netlifly.Shared.Response
{
    public class ChangePasswordResponse
    {
        public DataObject? Data { get; set; }

        public object? Errors { get; set; }

        public class DataObject
        {
            public OkData ChangePassword { get; set; }
        }
    }
}
