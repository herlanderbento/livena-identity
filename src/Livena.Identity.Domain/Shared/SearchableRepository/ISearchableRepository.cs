namespace Livena.Identity.Domain.Shared.SearchableRepository;

public interface ISearchableRepository<TAggregate, TSearch>
    where TAggregate : AggregateRoot
{
    Task<SearchOutput<TAggregate>> Search(
        SearchInput<TSearch> input,
        CancellationToken cancellationToken
    );
}
