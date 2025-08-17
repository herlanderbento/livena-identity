using Livena.Identity.Application.Exceptions;
using Livena.Identity.Application.Interfaces;
using Livena.Identity.Domain.Entity;
using Livena.Identity.Domain.Repository;

namespace Livena.Identity.Application.UseCases.User.Logout;

public class Logout : ILogout
{
    private readonly IRevokedTokenRepository _revokedTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IKeycloakService _keycloakService;

    public Logout(
        IRevokedTokenRepository revokedTokenRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IKeycloakService keycloakService
    )
    {
        _revokedTokenRepository = revokedTokenRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _keycloakService = keycloakService;
    }

    public async Task Handle(LogoutInput input, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsername(input.Username, cancellationToken);

        NotFoundException.ThrowIfNull(user, $"User {input.Username} not found.");

        var revokedToken = await _revokedTokenRepository.GetByToken(
            input.AccessToken,
            cancellationToken
        );

        ConflictException.ThrowIf(revokedToken != null, "Token already revoked.");

        var revoked = new RevokedToken(userId: user.Id, input.AccessToken, input.ExpiresAt);

        await _revokedTokenRepository.Insert(revoked, cancellationToken);

        await _keycloakService.Logout(input.Username, cancellationToken);

        await _unitOfWork.Commit(cancellationToken);
    }
}
