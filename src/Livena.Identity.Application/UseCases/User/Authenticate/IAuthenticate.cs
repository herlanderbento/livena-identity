using MediatR;

namespace Livena.Identity.Application.UseCases.User.Authenticate;

public interface IAuthenticate : IRequestHandler<AuthenticateInput, AuthenticateOutput>;
