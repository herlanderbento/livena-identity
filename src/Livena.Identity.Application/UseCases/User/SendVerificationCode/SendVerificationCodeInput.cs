using FluentValidation;
using Livena.Identity.Domain.Enum;
using MediatR;

namespace Livena.Identity.Application.UseCases.User.SendVerificationCode;

public class SendVerificationCodeInput : IRequest
{
    public required string Username { get; set; }

    public VerificationPurpose? Purpose { get; set; }

    public SendVerificationCodeInput(string username, VerificationPurpose purpose)
    {
        Username = username;
        Purpose = purpose;
    }

    public SendVerificationCodeInput() { }
}

public class SendVerificationCodeInputValidator : AbstractValidator<SendVerificationCodeInput>
{
    public SendVerificationCodeInputValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .NotEmpty()
            .WithMessage("Username is required.")
            .MaximumLength(255)
            .WithMessage("Username must not exceed 255 characters.");

        RuleFor(x => x.Purpose).IsInEnum().WithMessage("Invalid purpose.");
    }
}
