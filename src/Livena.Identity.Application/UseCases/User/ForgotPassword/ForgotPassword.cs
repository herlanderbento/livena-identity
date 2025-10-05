using Livena.Identity.Application.Exceptions;
using Livena.Identity.Application.Interfaces;
using Livena.Identity.Application.UseCases.User.Common;
using Livena.Identity.Domain.Enum;
using Livena.Identity.Domain.Repository;
using DomainEntity = Livena.Identity.Domain.Entity;

namespace Livena.Identity.Application.UseCases.User.ForgotPassword;

public class ForgotPassword : IForgotPassword
{
    private readonly IUserRepository _userRepository;
    private readonly IUserCodeRepository _userCodeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMailProvider _mailProvider;

    public ForgotPassword(
        IUserRepository userRepository,
        IUserCodeRepository userCodeRepository,
        IUnitOfWork unitOfWork,
        IMailProvider mailProvider
    )
    {
        _userRepository = userRepository;
        _userCodeRepository = userCodeRepository;
        _unitOfWork = unitOfWork;
        _mailProvider = mailProvider;
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

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            var url =
                $"https://app.livena.com/reset-password?code={Uri.EscapeDataString(userCode.Code)}&userId={user.Id}";

            var model = new TemplateModel
            {
                Username = user.Username,
                Url = url,
                Code = userCode.Code,
                ExpireMinutes = 15,
            };

            await _mailProvider.SendFromTemplateAsync(
                user.Email,
                "Reset your password",
                "ForgotPassword",
                model,
                cancellationToken
            );
        }
    }
}
