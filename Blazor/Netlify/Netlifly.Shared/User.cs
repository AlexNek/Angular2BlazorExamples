namespace Netlifly.Shared
{
    public class User
    {
        private const string DefaultLanguage = "en";

        public string Email { get; set; }

        public string Firstname { get; set; }

        public Hero[]? Heroes { get; set; }

        public string Id { get; set; }

        public string Language { get; set; } = DefaultLanguage;

        // eslint-disable-next-line complexity
        public User(User? user)
        {
            if (user != null)
            {
                Id = user.Id;
                Email = user.Email;
                Language = user.Language ?? DefaultLanguage;
                Firstname = user.Firstname;
                Heroes = user.Heroes;
            }
        }
    }
}
