using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netlifly.Shared
{
    // Translated from TypeScript to C#

    // Interfaces
    public interface JwtDecodeOptions
    {
        bool header { get; set; }
    }

    public interface JwtHeader
    {
        string typ { get; set; }
        string alg { get; set; }
        string kid { get; set; }
    }

    public interface JwtPayload
    {
        string iss { get; set; }
        string sub { get; set; }
        string[] aud { get; set; }
        int exp { get; set; }
        int nbf { get; set; }
        int iat { get; set; }
        string jti { get; set; }
    }

    // Functions
    //public T jwtDecode<T>(string token, JwtDecodeOptions options) where T : JwtHeader
    //{
    //    return default(T);
    //}

    //public T jwtDecode<T>(string token, JwtDecodeOptions options = null) where T : JwtPayload
    //{
    //    return default(T);
    //}
}
