using Livena.Identity.Application.Common;

namespace Livena.Identity.Api.Presenters;

public class ApiPresenterList<TItemData> : ApiPresenter<IReadOnlyList<TItemData>>
{
    public ApiPaginationPresenter Meta { get; private set; }

    public ApiPresenterList(int currentPage, int perPage, int total, IReadOnlyList<TItemData> data)
        : base(data)
    {
        Meta = new ApiPaginationPresenter(currentPage, perPage, total);
    }

    public ApiPresenterList(PaginatedListOutput<TItemData> paginatedListOutput)
        : base(paginatedListOutput.Items)
    {
        Meta = new ApiPaginationPresenter(
            paginatedListOutput.Page,
            paginatedListOutput.PerPage,
            paginatedListOutput.Total
        );
    }
}
