using Livena.Identity.Domain.Shared;

namespace Livena.Identity.Domain.Entity;

public class OAuthAccount: AggregateRoot
{
    public Guid UserId { get; private set; }
    public string Provider { get; private set; }
    public string ProviderUserId { get; private set; }
    public string AccessToken { get; private set; }
    public string RefreshToken { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    
    public OAuthAccount(
        Guid userId, 
        string provider, 
        string providerUserId, 
        string accessToken, 
        string refreshToken, 
        DateTime expiresAt): base()
    {
        UserId = userId;
        Provider = provider;
        ProviderUserId = providerUserId;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
    }
    
}