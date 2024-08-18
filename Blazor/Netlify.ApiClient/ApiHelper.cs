using GraphQL;

namespace Netlify.ApiClient;

internal static class ApiHelper
{
    public static void IfErrorThrowException(IGraphQLResponse response, object? errorHeader)
    {
        //Check if there are any errors in the GraphQL response
        if (response.Errors != null && response.Errors.Any())
        {
            // Handle the errors accordingly
            // Here we log the errors or you could throw an exception
            var errorMessages = string.Join(
                ", ",
                response.Errors.Select(e => e.Message));
            Console.WriteLine($"GraphQL errors occurred: {errorMessages}");

            // Optionally, you can throw an exception or return a special error object
            throw new GraphQLException($"{errorHeader} {errorMessages}");
        }
    }
}
