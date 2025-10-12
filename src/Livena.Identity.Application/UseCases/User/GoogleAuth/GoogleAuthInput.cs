using FluentValidation;
using Livena.Identity.Application.UseCases.User.Authenticate;
using MediatR;

namespace Livena.Identity.Application.UseCases.User.GoogleAuth;

public class GoogleAuthInput : IRequest<AuthenticateOutput>
{
    public string AuthorizationCode { get; set; } = null!;
    public string RedirectUri { get; set; } = null!;

    public GoogleAuthInput(string authorizationCode, string redirectUri)
    {
        AuthorizationCode = authorizationCode;
        RedirectUri = redirectUri;
    }
}

public class GoogleAuthInputValidator : AbstractValidator<GoogleAuthInput>
{
    public GoogleAuthInputValidator()
    {
        RuleFor(x => x.AuthorizationCode).NotEmpty().WithMessage("Authorization code is required");

        RuleFor(x => x.RedirectUri)
            .NotEmpty()
            .WithMessage("Redirect URI is required")
            .Must(BeValidUri)
            .WithMessage("Redirect URI must be a valid URI");
    }

    private static bool BeValidUri(string uri)
    {
        return Uri.TryCreate(uri, UriKind.Absolute, out _);
    }
}
