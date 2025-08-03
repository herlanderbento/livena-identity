using FluentValidation;
using MediatR;

namespace Livena.Identity.Application.UseCases.User.VerifyAccount;

public class VerifyAccountInput : IRequest
{
    public string Username { get; set; }
    public string Code { get; set; }

    public VerifyAccountInput(string username, string code)
    {
        Username = username;
        Code = code;
    }
}

public class VerifyAccountInputValidator : AbstractValidator<VerifyAccountInput>
{
    public VerifyAccountInputValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MaximumLength(255)
            .WithMessage("Username must not exceed 255 characters.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Code is required.")
            .MaximumLength(6)
            .WithMessage("Code must not exceed 6 characters.");
    }
}
