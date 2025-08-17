using FluentValidation;
using MediatR;

namespace Livena.Identity.Application.UseCases.User.ChangePassword;

public class ChangePasswordInput : IRequest
{
    public string Username { get; set; }
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }

    public ChangePasswordInput(string username, string currentPassword, string newPassword)
    {
        Username = username;
        CurrentPassword = currentPassword;
        NewPassword = newPassword;
    }
}

public class ChangePasswordInputValidator : AbstractValidator<ChangePasswordInput>
{
    public ChangePasswordInputValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("Current password is required.")
            .MinimumLength(6)
            .WithMessage("Current password must be at least 6 characters long.")
            .MaximumLength(255)
            .WithMessage("Current password must not exceed 255 characters.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required.")
            .MinimumLength(6)
            .WithMessage("New password must be at least 6 characters long.")
            .MaximumLength(255)
            .WithMessage("New password must not exceed 255 characters.")
            .NotEqual(x => x.CurrentPassword)
            .WithMessage("New password cannot be the same as the current password.");
    }
}
