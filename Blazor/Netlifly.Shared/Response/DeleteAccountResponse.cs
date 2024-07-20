namespace Netlifly.Shared.Response
{
    public class DeleteAccountResponse
    {
        public DataObject? Data { get; set; }

        public object? Errors { get; set; }

        public class DataObject
        {
            public OkData DeleteAccount { get; set; }
        }
    }
}
