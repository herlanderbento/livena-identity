using Livena.Identity.Application.Exceptions;
using Livena.Identity.Application.Interfaces;
using Livena.Identity.Domain.Enum;
using Livena.Identity.Domain.Repository;

namespace Livena.Identity.Application.UseCases.User.ResetPassword;

public class ResetPassword : IResetPassword
{
    private readonly IUserRepository _userRepository;
    private readonly IUserCodeRepository _userCodeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICryptography _cryptography;
    private readonly IKeycloakService _keycloakService;

    public ResetPassword(
        IUserRepository userRepository,
        IUserCodeRepository userCodeRepository,
        IUnitOfWork unitOfWork,
        ICryptography cryptography,
        IKeycloakService keycloakService
    )
    {
        _userRepository = userRepository;
        _userCodeRepository = userCodeRepository;
        _unitOfWork = unitOfWork;
        _cryptography = cryptography;
        _keycloakService = keycloakService;
    }

    public async Task Handle(ResetPasswordInput input, CancellationToken cancellationToken)
    {
        var userCode = await _userCodeRepository.GetByCode(input.Code, cancellationToken);

        if (
            userCode == null
            || userCode.Code != input.Code
            || userCode.Purpose != VerificationPurpose.PasswordReset
        )
        {
            throw new BadRequestException("Invalid verification code.");
        }

        BadRequestException.ThrowIf(userCode.IsExpired(), "Verification code expired.");

        BadRequestException.ThrowIf(userCode.IsUsed(), "Verification code already used.");

        var user = await _userRepository.GetById(userCode.UserId, cancellationToken);

        NotFoundException.ThrowIfNull(user, $"User {userCode.UserId} not found.");

        var hashedPassword = await _cryptography.HashPassword(input.Password, cancellationToken);

        user.ChangePassword(hashedPassword);

        await _userRepository.Update(user, cancellationToken);
        await _keycloakService.Update(user, input.Password, cancellationToken);

        userCode.Use();

        await _userCodeRepository.Update(userCode, cancellationToken);

        await _unitOfWork.Commit(cancellationToken);
    }
}
