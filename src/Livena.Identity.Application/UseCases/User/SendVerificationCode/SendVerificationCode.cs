using Livena.Identity.Application.Exceptions;
using Livena.Identity.Application.Interfaces;
using Livena.Identity.Application.UseCases.User.Common;
using Livena.Identity.Domain.Entity;
using Livena.Identity.Domain.Enum;
using Livena.Identity.Domain.Repository;

namespace Livena.Identity.Application.UseCases.User.SendVerificationCode;

public class SendVerificationCode : ISendVerificationCode
{
    private readonly IUserRepository _userRepository;
    private readonly IUserCodeRepository _userCodeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMailProvider _mailProvider;

    public SendVerificationCode(
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

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            var url =
                $"https://app.livena.com/verify-account?code={Uri.EscapeDataString(userCode.Code)}&userId={user.Id}";

            var model = new TemplateModel
            {
                Username = user.Username,
                Url = url,
                Code = userCode.Code,
                ExpireMinutes = 15,
            };

            await _mailProvider.SendFromTemplateAsync(
                user.Email,
                "Verify your account",
                "VerificationCode",
                model,
                cancellationToken
            );
        }
    }
}
