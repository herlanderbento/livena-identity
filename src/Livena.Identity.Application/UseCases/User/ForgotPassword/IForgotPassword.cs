using MediatR;

namespace Livena.Identity.Application.UseCases.User.ForgotPassword;

public interface IForgotPassword : IRequestHandler<ForgotPasswordInput>;
