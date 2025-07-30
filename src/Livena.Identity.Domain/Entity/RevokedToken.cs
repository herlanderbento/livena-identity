using Livena.Identity.Domain.Enum;
using Livena.Identity.Domain.Shared;
using Livena.Identity.Domain.Validator;

namespace Livena.Identity.Domain.Entity;

public class RevokedToken : AggregateRoot
{
    public Guid UserId { get; private set; }
    public string AccessToken { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime RevokedAt { get; private set; }

    public RevokedToken(Guid userId, string accessToken, DateTime expiresAt)
        : base()
    {
        UserId = userId;
        AccessToken = accessToken;
        ExpiresAt = DateTimeUtils.EnsureUtc(expiresAt);
        RevokedAt = DateTime.UtcNow;

        Validate();
    }

    private void Validate()
    {
        RevokedTokenValidator.Validate(UserId, AccessToken);
    }
}
