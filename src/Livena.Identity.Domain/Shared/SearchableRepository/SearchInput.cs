namespace Livena.Identity.Domain.Shared.SearchableRepository;

public class SearchInput<TSearch>
{
    public int Page { get; set; }
    public int PerPage { get; set; }
    public TSearch Search { get; set; }
    public string OrderBy { get; set; }
    public SearchOrder Order { get; set; }

    public SearchInput(
        int page, 
        int perPage, 
        TSearch search, 
        string orderBy, 
        SearchOrder order)
    {
        Page = page;
        PerPage = perPage;
        Search = search;
        OrderBy = orderBy;
        Order = order;
    }
}