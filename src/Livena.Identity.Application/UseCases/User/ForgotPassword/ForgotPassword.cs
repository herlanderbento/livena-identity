using Livena.Identity.Application.Exceptions;
using Livena.Identity.Application.Interfaces;
using Livena.Identity.Domain.Enum;
using Livena.Identity.Domain.Repository;
using DomainEntity = Livena.Identity.Domain.Entity;

namespace Livena.Identity.Application.UseCases.User.ForgotPassword;

public class ForgotPassword : IForgotPassword
{
    private readonly IUserRepository _userRepository;
    private readonly IUserCodeRepository _userCodeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ForgotPassword(
        IUserRepository userRepository,
        IUserCodeRepository userCodeRepository,
        IUnitOfWork unitOfWork
    )
    {
        _userRepository = userRepository;
        _userCodeRepository = userCodeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ForgotPasswordInput input, CancellationToken cancellationToken)
    {
        DomainEntity.User? user = null;

        if (!string.IsNullOrWhiteSpace(input.Email))
        {
            user = await _userRepository.GetByEmail(input.Email, cancellationToken);
            NotFoundException.ThrowIfNull(user, $"Email '{input.Email}' not found.");
        }

        if (!string.IsNullOrWhiteSpace(input.Phone))
        {
            user = await _userRepository.GetByPhone(input.Phone, cancellationToken);
            NotFoundException.ThrowIfNull(user, $"Phone '{input.Phone}' not found.");
        }

        var userCode = new DomainEntity.UserCode(user!.Id, VerificationPurpose.PasswordReset);

        await _userCodeRepository.Insert(userCode, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
    }
}
