using Livena.Identity.Application.Common;
using Livena.Identity.Domain.Shared.SearchableRepository;
using MediatR;

namespace Livena.Identity.Application.UseCases.User.ListUsers;

public class ListUsersInput : PaginatedListInput<string>, IRequest<ListUsersOutput>
{
    public ListUsersInput(
        int page = 1,
        int perPage = 15,
        string search = "",
        string sort = "",
        SearchOrder dir = SearchOrder.Asc
    )
        : base(page, perPage, search, sort, dir) { }

    public ListUsersInput()
        : base(1, 15, "", "", SearchOrder.Asc) { }
}
