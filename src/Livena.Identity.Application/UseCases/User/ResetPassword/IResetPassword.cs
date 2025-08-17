using MediatR;

namespace Livena.Identity.Application.UseCases.User.ResetPassword;

public interface IResetPassword : IRequestHandler<ResetPasswordInput>;
