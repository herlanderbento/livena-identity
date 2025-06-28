namespace Livena.Identity.Application.Interfaces;

public class KeycloakTokenResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public int ExpiresIn { get; set; }
}

public interface IKeycloakService
{
    Task Insert(object input, CancellationToken cancellationToken);
    Task Delete(string keycloakUserId, CancellationToken cancellationToken);
    Task Update(string keycloakUserId, object input, CancellationToken cancellationToken);
}