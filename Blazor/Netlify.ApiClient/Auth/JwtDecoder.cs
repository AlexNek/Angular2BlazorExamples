using System.IdentityModel.Tokens.Jwt;

using Newtonsoft.Json;

namespace Netlify.ApiClient.Auth;

public class JwtDecoder
{
    public static string? Decode(string tokenStr)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(tokenStr);
        if (token != null)
        {
            var ret = JsonConvert.SerializeObject(token);
            return ret;
        }
        return null;
    }
}
