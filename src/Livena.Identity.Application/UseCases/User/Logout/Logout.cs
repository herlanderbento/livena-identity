using Livena.Identity.Application.Exceptions;
using Livena.Identity.Application.Interfaces;
using Livena.Identity.Domain.Entity;
using Livena.Identity.Domain.Repository;

namespace Livena.Identity.Application.UseCases.User.Logout;

public class Logout : ILogout
{
    private readonly IRevokedTokenRepository _revokedTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IKeycloakService _keycloakService;

    public Logout(
        IRevokedTokenRepository revokedTokenRepository,
        IUnitOfWork unitOfWork,
        IKeycloakService keycloakService
    )
    {
        _revokedTokenRepository = revokedTokenRepository;
        _unitOfWork = unitOfWork;
        _keycloakService = keycloakService;
    }

    public async Task Handle(LogoutInput input, CancellationToken cancellationToken)
    {
        var user = await _keycloakService.GetKeycloakIdByUsername(
            input.Username,
            cancellationToken
        );

        NotFoundException.ThrowIfNull(user, $"User {user} not found.");

        var revoked = new RevokedToken(
            userId: Guid.Parse(user!),
            input.AccessToken,
            input.ExpiresAt
        );

        await _revokedTokenRepository.Insert(revoked, cancellationToken);

        if (user != null)
        {
            await _keycloakService.Logout(user, cancellationToken);
        }

        await _unitOfWork.Commit(cancellationToken);
    }
}
