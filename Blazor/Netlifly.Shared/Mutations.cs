using GraphQL;

namespace Netlifly.Shared
{
    internal static class Mutations
    {
        public static readonly GraphQLRequest ChangePasswordMutation =
            new GraphQLRequest
                {
                    Query = @"
        mutation changePassword($oldPassword: String!, $newPassword: String!) {
            changePassword(data: { oldPassword: $oldPassword, newPassword: $newPassword }) {
                id
            }
        }",
                    Variables = new { oldPassword = "", newPassword = "" }
                };

        public static readonly GraphQLRequest DeleteAccountMutation =
            new GraphQLRequest
                {
                    Query = @"
        mutation deleteAccount($password: String!) {
            deleteAccount(password: $password) {
                ok
            }
        }",
                    Variables = new { password = "" }
                };

        public static readonly GraphQLRequest LoginMutation
            = new GraphQLRequest
                  {
                      Query = @"
        mutation login($email: String!, $password: String!) {
            login(data: { email: $email, password: $password }) {
                accessToken
                refreshToken
                user {
                    id
                    email
                    firstname
                    language
                    heroes {
                        id
                        realName
                        alterEgo
                        image
                    }
                }
            }
        }",
                      Variables = new { email = "", password = "" }
                  };

        public static readonly GraphQLRequest RefreshTokenMutation =
            new GraphQLRequest
                {
                    Query = @"
        mutation refreshToken($refreshToken: String!) {
            refreshToken(token: $refreshToken) {
                accessToken
                refreshToken
            }
        }",
                    Variables = new { refreshToken = "" }
                };

        public static readonly GraphQLRequest SignupMutation =
            new GraphQLRequest
                {
                    Query = @"
        mutation signup($firstname: String!, $email: String!, $password: String!) {
            signup(data: { firstname: $firstname, email: $email, password: $password }) {
                accessToken
                refreshToken
                user {
                    id
                    email
                    firstname
                    language
                }
            }
        }",
                    Variables = new { firstname = "", email = "", password = "" }
                };

        public static readonly GraphQLRequest UpdateUserMutation =
            new GraphQLRequest
                {
                    Query = @"
        mutation updateUser($firstname: String!, $language: String!) {
            updateUser(data: { firstname: $firstname, language: $language }) {
                id
                firstname
                email
                language
            }
        }",
                    Variables = new { firstname = "", language = "" }
                };
    }
}
