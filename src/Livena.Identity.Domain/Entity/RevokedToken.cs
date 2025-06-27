using Livena.Identity.Domain.Enum;
using Livena.Identity.Domain.Shared;
using Livena.Identity.Domain.Validator;

namespace Livena.Identity.Domain.Entity;

public class RevokedToken: AggregateRoot
{
    public Guid UserId { get; private set; }
    public string AccessToken { get; private set; }
    public string RefreshToken { get; private set; }
    public TokenTypes TokenType { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime RevokedAt { get; private set; }
    
    public RevokedToken(
        Guid userId, 
        string accessToken, 
        string refreshToken, 
        TokenTypes tokenType, 
        DateTime expiresAt): base()
    {
        UserId = userId;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        TokenType = tokenType;
        ExpiresAt = expiresAt;
        RevokedAt = DateTime.UtcNow;
    }
    
    private void Validate()
    {
        RevokedTokenValidator.Validate(
            UserId,
            AccessToken,
            RefreshToken
        );
    }
}