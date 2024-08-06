namespace Netlify.ApiClient
{
    public class GraphQLException : Exception
    {
        public GraphQLException(string message):base(message)
        {
        }
    }
}
