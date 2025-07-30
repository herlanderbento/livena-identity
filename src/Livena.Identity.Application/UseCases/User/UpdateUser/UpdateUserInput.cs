using FluentValidation;
using Livena.Identity.Application.UseCases.User.Common;
using MediatR;

namespace Livena.Identity.Application.UseCases.User.UpdateUser;

public class UpdateUserInput : IRequest<UserOutput>
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? Birthday { get; set; }

    public UpdateUserInput(
        Guid id,
        string? email,
        string? phone,
        DateTime? birthday,
        bool? isActive
    )
    {
        Id = id;
        Email = email;
        Phone = phone;
        Birthday = birthday;
        IsActive = isActive;
    }
}

public class UpdateUserInputValidator : AbstractValidator<UpdateUserInput>
{
    public UpdateUserInputValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("ID is required.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Birthday)
            .LessThan(DateTime.Today)
            .WithMessage("Birthday must be in the past.")
            .When(x => x.Birthday.HasValue);

        RuleFor(x => x.Phone)
            .Matches(@"^\+?[1-9]\d{7,14}$")
            .WithMessage("Invalid phone number format.")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));
    }
}
