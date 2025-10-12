using Livena.Identity.Domain.Entity;

namespace Livena.Identity.Application.Interfaces;

public class KeycloakTokenResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public int ExpiresIn { get; set; }
    public string? IdToken { get; set; }
}

public interface IKeycloakService
{
    Task Insert(User user, string password, CancellationToken cancellationToken);
    Task Update(User user, string? password, CancellationToken cancellationToken);
    Task<string?> GetKeycloakIdByUsername(string username, CancellationToken cancellationToken);
    Task Delete(string keycloakUserId, CancellationToken cancellationToken);
    Task AssignUserRoleDirectly(string keycloakUserId, CancellationToken cancellationToken);
    Task<string?> GetRoleIdByName(string roleName, CancellationToken cancellationToken);
    Task<KeycloakTokenResponse> Login(
        string username,
        string password,
        CancellationToken cancellationToken
    );
    Task Logout(string username, CancellationToken cancellationToken);
}
