using Livena.Identity.Application.Exceptions;
using Livena.Identity.Domain.Entity;
using Livena.Identity.Domain.Repository;
using Livena.Identity.Domain.Shared.SearchableRepository;
using Microsoft.EntityFrameworkCore;

namespace Livena.Identity.infra.EntityFramework.Repositories;

public class RevokedTokenRepository(LivenaIdentityDbContext context) : IRevokedTokenRepository
{
    private readonly LivenaIdentityDbContext _context = context;
    private DbSet<RevokedToken> RevokedTokens => _context.Set<RevokedToken>();

    public async Task Insert(RevokedToken aggregate, CancellationToken cancellationToken) =>
        await RevokedTokens.AddAsync(aggregate, cancellationToken);

    public async Task<RevokedToken> GetById(Guid id, CancellationToken cancellationToken)
    {
        var model = await RevokedTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        NotFoundException.ThrowIfNull(model, $"RevokedToken '{id}' not found.");
        return model!;
    }

    public async Task<SearchOutput<RevokedToken>> Search(
        SearchInput<string> input,
        CancellationToken cancellationToken
    )
    {
        var toSkip = (input.Page - 1) * input.PerPage;
        var query = RevokedTokens.AsNoTracking();
        query = AddOrderToQuery(query, input.OrderBy, input.Order);

        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip(toSkip).Take(input.PerPage).ToListAsync(cancellationToken);

        return new SearchOutput<RevokedToken>(input.Page, input.PerPage, total, items);
    }

    public Task Update(RevokedToken aggregate, CancellationToken _) =>
        Task.FromResult(RevokedTokens.Update(aggregate));

    public Task Delete(RevokedToken aggregate, CancellationToken _) =>
        Task.FromResult(RevokedTokens.Remove(aggregate));

    private IQueryable<RevokedToken> AddOrderToQuery(
        IQueryable<RevokedToken> query,
        string orderProperty,
        SearchOrder order
    )
    {
        return (orderProperty.ToLower(), order) switch
        {
            ("id", SearchOrder.Asc) => query.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => query.OrderByDescending(x => x.Id),
            ("createdAt", SearchOrder.Asc) => query.OrderBy(x => x.RevokedAt),
            ("createdAt", SearchOrder.Desc) => query.OrderByDescending(x => x.RevokedAt),
            _ => query.OrderBy(x => x.RevokedAt).ThenBy(x => x.Id),
        };
    }
}
