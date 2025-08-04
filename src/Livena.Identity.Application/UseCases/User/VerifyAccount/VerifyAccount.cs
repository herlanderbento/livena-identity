using Livena.Identity.Application.Exceptions;
using Livena.Identity.Application.Interfaces;
using Livena.Identity.Domain.Repository;

namespace Livena.Identity.Application.UseCases.User.VerifyAccount;

public class VerifyAccount : IVerifyAccount
{
    private readonly IUserRepository _userRepository;
    private readonly IUserCodeRepository _userCodeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VerifyAccount(
        IUserRepository userRepository,
        IUserCodeRepository userCodeRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _userCodeRepository = userCodeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(VerifyAccountInput input, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsername(input.Username, cancellationToken);

        NotFoundException.ThrowIfNull(user, $"User {input.Username} not fount.");

        var userCode = await _userCodeRepository.GetByUserId(user.Id, cancellationToken);

        BadRequestException.ThrowIf(
            userCode == null || userCode.Code != input.Code,
            "Invalid verification code."
        );

        BadRequestException.ThrowIf(userCode!.IsExpired(), "Verification code expired.");

        ConflictException.ThrowIf(user.IsVerified == true, "User already verified.");
    
        user.Verify();

        await _userRepository.Update(user, cancellationToken);

        userCode.Use();

        await _userCodeRepository.Update(userCode, cancellationToken);

        await _unitOfWork.Commit(cancellationToken);
    }
}
