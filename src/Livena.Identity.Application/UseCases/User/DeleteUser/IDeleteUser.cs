using MediatR;

namespace Livena.Identity.Application.UseCases.User.DeleteUser;

public interface IDeleteUser : IRequestHandler<DeleteUserInput>;
