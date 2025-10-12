using Livena.Identity.Application.UseCases.User.Authenticate;
using MediatR;

namespace Livena.Identity.Application.UseCases.User.GoogleAuth;

public interface IGoogleAuth : IRequestHandler<GoogleAuthInput, AuthenticateOutput>;
