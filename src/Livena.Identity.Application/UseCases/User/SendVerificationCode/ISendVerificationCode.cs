using MediatR;

namespace Livena.Identity.Application.UseCases.User.SendVerificationCode;

public interface ISendVerificationCode : IRequestHandler<SendVerificationCodeInput>;
