using FluentValidation;
using MediatR;

namespace Livena.Identity.Application.UseCases.User.SendVerifyCode;

public class SendVerifyCodeInput(string username) : IRequest
{
    public string Username { get; set; } = username;
}

public class SendVerifyCodeInputValidator : AbstractValidator<SendVerifyCodeInput>
{
    public SendVerifyCodeInputValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MaximumLength(255)
            .WithMessage("Username must not exceed 255 characters.");
    }
}
