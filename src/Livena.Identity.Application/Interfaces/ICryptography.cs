namespace Livena.Identity.Application.Interfaces;

public interface ICryptography
{
    Task<string> HashPassword(string password, CancellationToken cancellationToken);
    Task<bool> Verify(string password, string passwordHash, CancellationToken cancellationToken);
}