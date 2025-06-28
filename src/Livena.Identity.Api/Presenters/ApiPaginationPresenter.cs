namespace Livena.Identity.Api.Presenters;

public class ApiPaginationPresenter
{
    public int CurrentPage { get; set; }
    public int PerPage { get; set; }
    public int Total { get; set; }
    public int LastPage { get; private set; }

    public ApiPaginationPresenter(int currentPage, int perPage, int total)
    {
        CurrentPage = currentPage;
        PerPage = perPage;
        Total = total;
        LastPage = (int)Math.Ceiling((double)Total / PerPage);
    }
}
