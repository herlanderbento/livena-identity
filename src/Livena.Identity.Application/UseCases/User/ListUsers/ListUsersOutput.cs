using Livena.Identity.Application.Common;
using Livena.Identity.Application.UseCases.User.Common;

namespace Livena.Identity.Application.UseCases.User.ListUsers;

public class ListUsersOutput(int page, int perPage, int total, IReadOnlyList<UserOutput> items)
    : PaginatedListOutput<UserOutput>(page, perPage, total, items);
