using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Netlifly.Shared.Request;

namespace Netlifly.Shared.Response;

public class RefreshTokenResponse
{
    public UpdateTokenData RefreshToken { get; set; }
}