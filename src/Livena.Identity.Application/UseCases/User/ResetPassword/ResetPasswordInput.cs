using FluentValidation;
using MediatR;

namespace Livena.Identity.Application.UseCases.User.ResetPassword;

public class ResetPasswordInput : IRequest
{
    public required string Code { get; set; }
    public required string Password { get; set; }

    public ResetPasswordInput(string code, string password)
    {
        Code = code;
        Password = password;
    }
}

public class ResetPasswordInputValidator : AbstractValidator<ResetPasswordInput>
{
    public ResetPasswordInputValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Code is required.")
            .MaximumLength(6)
            .WithMessage("Code must not exceed 6 characters.");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long.")
            .MaximumLength(255)
            .WithMessage("Password must not exceed 255 characters.");
    }
}
