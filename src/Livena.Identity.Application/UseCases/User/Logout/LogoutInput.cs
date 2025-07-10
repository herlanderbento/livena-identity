using FluentValidation;
using MediatR;

namespace Livena.Identity.Application.UseCases.User.Logout;

public class LogoutInput : IRequest
{
    public string Username { get; set; }
    public string AccessToken { get; set; }
    public DateTime ExpiresAt { get; set; }

    public LogoutInput(string username, string accessToken, DateTime expiresAt)
    {
        Username = username;
        AccessToken = accessToken;
        ExpiresAt = expiresAt;
    }
}

public class LogoutInputValidator : AbstractValidator<LogoutInput>
{
    public LogoutInputValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("AccessToken is required.");
        
        RuleFor(x => x.ExpiresAt)
            .NotEmpty().WithMessage("ExpiresAt is required.")
            .Must(date => date != default).WithMessage("ExpiresAt is required.");
    }
}
