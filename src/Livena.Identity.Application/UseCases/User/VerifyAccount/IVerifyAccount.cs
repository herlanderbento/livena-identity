using MediatR;

namespace Livena.Identity.Application.UseCases.User.VerifyAccount;

public interface IVerifyAccount : IRequestHandler<VerifyAccountInput>;
