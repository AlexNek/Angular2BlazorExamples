namespace Netlify.ApiClient.Users
{
    internal static class UserQueries
    {
        public const string GetMe = @"
            query me {
                me {
                    id
                    email
                    firstname
                    heroes {
                        id
                        realName
                        alterEgo
                    }
                }
            }";
    }
}
