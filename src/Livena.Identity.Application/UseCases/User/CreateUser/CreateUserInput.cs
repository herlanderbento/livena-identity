using FluentValidation;
using Livena.Identity.Application.UseCases.User.Common;
using Livena.Identity.Domain.Enum;
using MediatR;

namespace Livena.Identity.Application.UseCases.User.CreateUser;

public class CreateUserInput : IRequest<UserOutput>
{
    public string Username { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Password { get; set; }
    public DateTime Birthday { get; set; }

    public CreateUserInput(
        string username,
        string password,
        DateTime birthday,
        string? email = null,
        string? phone = null
    )
    {
        Username = username;
        Password = password;
        Email = email;
        Phone = phone;
        Birthday = birthday;
    }
}

public class CreateUserInputValidator : AbstractValidator<CreateUserInput>
{
    public CreateUserInputValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MaximumLength(255)
            .WithMessage("Username must not exceed 255 characters.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long.")
            .MaximumLength(20)
            .WithMessage("Password must not exceed 20 characters.");

        RuleFor(x => x.Birthday)
            .NotEmpty()
            .WithMessage("Birthday is required.")
            .Must(date => date != default)
            .WithMessage("Birthday is required.")
            .LessThan(DateTime.Today)
            .WithMessage("Birthday must be in the past.");

        RuleFor(x => x)
            .Custom(
                (input, context) =>
                {
                    var hasEmail = !string.IsNullOrWhiteSpace(input.Email);
                    var hasPhone = !string.IsNullOrWhiteSpace(input.Phone);

                    if (!hasEmail && !hasPhone)
                    {
                        context.AddFailure("You must provide either Email or Phone.");
                    }
                    else if (hasEmail && hasPhone)
                    {
                        context.AddFailure(
                            "You must provide only one: either Email or Phone, not both."
                        );
                    }
                }
            );

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Phone)
            .Matches(@"^\+?[1-9]\d{7,14}$")
            .WithMessage("Invalid phone number format.")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));
    }
}
