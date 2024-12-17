using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace Netlify.Helpers
{
    public static class TokenHelper
    {
        public static (bool IsAccessTokenExpired, bool IsRefreshTokenExpired) GetTokenExpirationsState(
            string accessToken,
            string refreshToken)
        {
            var accessTokenExpiry = GetTokenExpiry(accessToken);
            var refreshTokenExpiry = GetTokenExpiry(refreshToken);

            var isAccessTokenExpired = DateTime.UtcNow >= accessTokenExpiry;
            var isRefreshTokenExpired = DateTime.UtcNow >= refreshTokenExpiry;

            return (isAccessTokenExpired, isRefreshTokenExpired);
        }
        public static (DateTime accessTokenExpiry, DateTime refreshTokenExpiry) GetTokenExpirationsDate(
            string accessToken,
            string refreshToken)
        {
            var accessTokenExpiry = GetTokenExpiry(accessToken);
            var refreshTokenExpiry = GetTokenExpiry(refreshToken);

            return (accessTokenExpiry, refreshTokenExpiry);
        }

        public static bool IsTokenNearExpired(string token, TimeSpan threshold)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            try
            {
                var tokenExpiry = GetTokenExpiry(token);

                var currentTime = DateTime.UtcNow;
                var timeUntilExpiration = tokenExpiry - currentTime;

                return timeUntilExpiration <= threshold;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error parsing token: {ex.Message}");
                return true; // Assume token is near expiration if we can't parse it
            }
        }

        private static DateTime GetTokenExpiry(string token)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
            var expiry = jwtToken?.ValidTo;
            return expiry ?? DateTime.MinValue;
        }
    }
}
