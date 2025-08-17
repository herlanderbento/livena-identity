using Livena.Identity.Application.Exceptions;
using Livena.Identity.Application.Interfaces;
using Livena.Identity.Domain.Repository;

namespace Livena.Identity.Application.UseCases.User.ChangePassword;

public class ChangePassword : IChangePassword
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICryptography _cryptography;
    private readonly IKeycloakService _keycloakService;

    public ChangePassword(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ICryptography cryptography,
        IKeycloakService keycloakService
    )
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _cryptography = cryptography;
        _keycloakService = keycloakService;
    }

    public async Task Handle(ChangePasswordInput input, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsername(input.Username, cancellationToken);

        NotFoundException.ThrowIfNull(user, $"User {input.Username} not found.");

        var hashedCurrentPassword = await _cryptography.Verify(
            input.CurrentPassword,
            user.Password,
            cancellationToken
        );

        ConflictException.ThrowIf(!hashedCurrentPassword, "Current password does not match.");

        var hashedNewPassword = await _cryptography.HashPassword(
            input.NewPassword,
            cancellationToken
        );

        user.ChangePassword(hashedNewPassword);

        await _userRepository.Update(user, cancellationToken);
        await _keycloakService.Update(user, input.NewPassword, cancellationToken);

        await _unitOfWork.Commit(cancellationToken);
    }
}
