using MediatR;

namespace Livena.Identity.Application.UseCases.User.ChangePassword;

public interface IChangePassword : IRequestHandler<ChangePasswordInput>;
