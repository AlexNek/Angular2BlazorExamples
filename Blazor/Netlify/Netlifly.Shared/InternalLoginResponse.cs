using System.Security.Claims;

namespace Netlifly.Shared;

public class InternalLoginResponse
{
    /// <summary>
    /// Gets or sets the claims. Default (MS) Claim has problem with serialization
    /// </summary>
    /// <value>The claims.</value>
    public List<ClaimInfo> Claims { get; set; }

    public bool IsAuthenticated { get; set; }

    public string Name { get; set; }

    public class ClaimInfo
    {
        public string Type { get; set; }

        public string Value { get; set; }
    }
}