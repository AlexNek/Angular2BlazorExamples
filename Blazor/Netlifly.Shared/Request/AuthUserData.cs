namespace Netlifly.Shared.Request
{
    public class AuthUserData
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public User User { get; set; }
    }
}
