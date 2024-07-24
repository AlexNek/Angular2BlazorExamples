namespace Netlifly.Shared
{
    public class AuthProps
    {
        public string? AccessToken { get; set; }

        public string? RefreshToken { get; set; }

        public User? User { get; set; }
    }
}
