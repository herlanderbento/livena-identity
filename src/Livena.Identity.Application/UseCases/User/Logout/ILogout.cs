using MediatR;

namespace Livena.Identity.Application.UseCases.User.Logout;

public interface ILogout : IRequestHandler<LogoutInput>;
