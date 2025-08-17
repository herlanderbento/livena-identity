using FluentValidation;
using MediatR;

namespace Livena.Identity.Application.UseCases.User.ForgotPassword;

public class ForgotPasswordInput : IRequest
{
    public string? Email { get; set; }
    public string? Phone { get; set; }

    public ForgotPasswordInput(string? email, string? phone)
    {
        Email = email;
        Phone = phone;
    }

    public ForgotPasswordInput() { }
}

public class ForgotPasswordInputValidator : AbstractValidator<ForgotPasswordInput>
{
    public ForgotPasswordInputValidator()
    {
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
