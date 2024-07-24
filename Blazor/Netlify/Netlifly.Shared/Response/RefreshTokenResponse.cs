using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Netlifly.Shared.Request;

namespace Netlifly.Shared.Response
{
    // Translated from TypeScript to C#

    public class RefreshTokenResponse
    {
        public object? Errors { get; set; }
        public RefreshTokenData? Data { get; set; }

        public class RefreshTokenData
        {
            public UpdateTokenData RefreshToken { get; set; }
        }
        //public class RefreshTokenInfo
        //{
        //    public string AccessToken { get; set; }
        //    public string RefreshToken { get; set; }
        //}

    }

    

    
}
