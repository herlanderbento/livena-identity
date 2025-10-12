using Livena.Identity.Domain.Entity;

namespace Livena.Identity.Application.Interfaces;

public class OAuthUserInfo
{
    public string Provider { get; set; } = null!;
    public string ProviderUserId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Picture { get; set; }
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public DateTime? Birthday { get; set; }
}

public interface IOAuthService
{
    public Task<OAuthUserInfo> GetUserInfo(
        string authorizationCode,
        string redirectUri,
        CancellationToken cancellationToken
    );
    public Task<KeycloakTokenResponse> ExchangeCodeForTokens(
        string authorizationCode,
        string redirectUri,
        CancellationToken cancellationToken
    );
    public Task<User> FindUserByOAuthAccount(
        string provider,
        string providerUserId,
        CancellationToken cancellationToken
    );
    public Task<OAuthAccount> CreateOAuthAccount(
        Guid userId,
        OAuthUserInfo userInfo,
        CancellationToken cancellationToken
    );
    public Task<OAuthAccount?> GetOAuthAccount(
        Guid userId,
        string provider,
        CancellationToken cancellationToken
    );
    public Task UpdateOAuthAccount(
        OAuthAccount oAuthAccount,
        OAuthUserInfo userInfo,
        CancellationToken cancellationToken
    );
}
