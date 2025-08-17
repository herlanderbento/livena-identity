using System.Security.Cryptography;
using Livena.Identity.Domain.Enum;
using Livena.Identity.Domain.Shared;

namespace Livena.Identity.Domain.Entity;

public class UserCode : AggregateRoot
{
    public Guid UserId { get; private set; }
    public string Code { get; private set; }
    public VerificationPurpose Purpose { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? UsedAt { get; private set; }

    public UserCode(Guid userId, VerificationPurpose purpose, DateTime? usedAt = null)
        : base()
    {
        UserId = userId;
        Code = GenerateCode();
        Purpose = purpose;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = DateTime.UtcNow.AddMinutes(15);
        UsedAt = usedAt;
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow >= ExpiresAt;
    }

    public bool IsUsed()
    {
        return UsedAt != null;
    }

    public void Use()
    {
        UsedAt = DateTime.UtcNow;
    }

    private static string GenerateCode(int length = 6)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var bytes = new byte[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }

        var result = new char[length];
        for (var i = 0; i < length; i++)
        {
            var idx = bytes[i] % chars.Length;
            result[i] = chars[idx];
        }

        return new string(result);
    }
}
