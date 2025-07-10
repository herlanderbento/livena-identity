using System.Security.Claims;

namespace Livena.Identity.Api.Helpers;

public static class JwtTokenHelper
{
    public static (string username, string accessToken, DateTime expiresAt) ExtractTokenInfo(ClaimsPrincipal user, string authorizationHeader)
    {
        // Get username from preferred_username claim
        var usernameClaim = user.FindFirst("preferred_username");
        if (usernameClaim == null)
        {
            throw new InvalidOperationException("Username not found in token (preferred_username claim missing)");
        }
        var username = usernameClaim.Value;

        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            throw new InvalidOperationException("Invalid authorization header");
        }
        var accessToken = authorizationHeader.Substring("Bearer ".Length);

        var expClaim = user.FindFirst("exp");
        if (expClaim == null || !long.TryParse(expClaim.Value, out var expTimestamp))
        {
            throw new InvalidOperationException("Invalid expiration time in token");
        }
        var expiresAt = DateTimeOffset.FromUnixTimeSeconds(expTimestamp).UtcDateTime;

        return (username, accessToken, expiresAt);
    }
} 