using Livena.Identity.Domain.Entity;

namespace Livena.Identity.Application.Interfaces;

public class KeycloakTokenResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public int ExpiresIn { get; set; }
}

public interface IKeycloakService
{
    Task Insert(User user, string password, CancellationToken cancellationToken);
    Task Delete(string keycloakUserId, CancellationToken cancellationToken);
    Task Update(User user, CancellationToken cancellationToken);
    Task<object?> GetById(string keycloakUserId, CancellationToken cancellationToken);
    Task<KeycloakTokenResponse> Login(string username, string password,CancellationToken cancellationToken);
    Task<string?> GetKeycloakIdByExternalId(string externalId, CancellationToken cancellationToken);
}