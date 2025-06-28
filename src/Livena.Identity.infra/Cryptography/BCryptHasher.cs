using Livena.Identity.Application.Interfaces;

namespace Livena.Identity.infra.Cryptography;

public class BCryptHasher: ICryptography
{
    private const int WorkFactor  = 10;

    public Task<string> HashPassword(string password, CancellationToken cancellationToken)
    {
        return Task.FromResult(BCrypt.Net.BCrypt.HashPassword(password, WorkFactor));
    }

    public Task<bool> Verify(string password, string passwordHash, CancellationToken cancellationToken)
    {
        return Task.FromResult(BCrypt.Net.BCrypt.Verify(password, passwordHash));
    }
}