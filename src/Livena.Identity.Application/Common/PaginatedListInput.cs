using Livena.Identity.Domain.Shared.SearchableRepository;

namespace Livena.Identity.Application.Common;

public abstract class PaginatedListInput<TSearch>
{
    public int Page { get; set; }
    public int PerPage { get; set; }
    public TSearch Search { get; set; }
    public string Sort { get; set; }
    public SearchOrder Dir { get; set; }
    
    public PaginatedListInput(
        int page, 
        int perPage, 
        TSearch search, 
        string sort, 
        SearchOrder dir)
    {
        Page = page;
        PerPage = perPage;
        Search = search;
        Sort = sort;
        Dir = dir;
    }

    public SearchInput<TSearch> ToSearchInput()
        => new(Page, PerPage, Search, Sort, Dir);
    
}