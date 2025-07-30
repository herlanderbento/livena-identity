using MediatR;

namespace Livena.Identity.Application.UseCases.User.ListUsers;

public interface IListUsers : IRequestHandler<ListUsersInput, ListUsersOutput>;
