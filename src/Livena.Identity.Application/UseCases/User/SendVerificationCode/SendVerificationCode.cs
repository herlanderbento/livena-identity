using Livena.Identity.Application.Exceptions;
using Livena.Identity.Application.Interfaces;
using Livena.Identity.Domain.Entity;
using Livena.Identity.Domain.Enum;
using Livena.Identity.Domain.Repository;

namespace Livena.Identity.Application.UseCases.User.SendVerificationCode;

public class SendVerificationCode : ISendVerificationCode
{
    private readonly IUserRepository _userRepository;
    private readonly IUserCodeRepository _userCodeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SendVerificationCode(
        IUserRepository userRepository,
        IUserCodeRepository userCodeRepository,
        IUnitOfWork unitOfWork
    )
    {
        _userRepository = userRepository;
        _userCodeRepository = userCodeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SendVerificationCodeInput input, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsername(input.Username, cancellationToken);

        NotFoundException.ThrowIfNull(user, $"User with username {input.Username} not found.");

        var userCode = new UserCode(
            user.Id,
            input.Purpose ?? VerificationPurpose.AccountVerification
        );

        await _userCodeRepository.Insert(userCode, cancellationToken);

        await _unitOfWork.Commit(cancellationToken);
    }
}
