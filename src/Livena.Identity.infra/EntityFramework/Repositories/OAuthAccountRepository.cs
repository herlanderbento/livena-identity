using Livena.Identity.Domain.Entity;
using Livena.Identity.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Livena.Identity.infra.EntityFramework.Repositories;

public class OAuthAccountRepository : IOAuthAccountRepository
{
    private readonly LivenaIdentityDbContext _context;

    public OAuthAccountRepository(LivenaIdentityDbContext context)
    {
        _context = context;
    }

    public async Task Insert(OAuthAccount oAuthAccount, CancellationToken cancellationToken)
    {
        await _context.OAuthAccounts.AddAsync(oAuthAccount, cancellationToken);
    }

    public async Task<OAuthAccount> GetById(Guid id, CancellationToken cancellationToken)
    {
        var model = await _context.OAuthAccounts.FirstOrDefaultAsync(
            oa => oa.Id == id,
            cancellationToken
        );

        return model!;
    }

    public async Task<OAuthAccount?> GetByUserIdAndProvider(
        Guid userId,
        string provider,
        CancellationToken cancellationToken
    )
    {
        return await _context.OAuthAccounts.FirstOrDefaultAsync(
            oa => oa.UserId == userId && oa.Provider == provider,
            cancellationToken
        );
    }

    public async Task<OAuthAccount?> GetByProviderAndProviderUserId(
        string provider,
        string providerUserId,
        CancellationToken cancellationToken
    )
    {
        return await _context.OAuthAccounts.FirstOrDefaultAsync(
            oa => oa.Provider == provider && oa.ProviderUserId == providerUserId,
            cancellationToken
        );
    }

    public async Task Update(OAuthAccount oAuthAccount, CancellationToken cancellationToken)
    {
        _context.OAuthAccounts.Update(oAuthAccount);
        await Task.CompletedTask;
    }

    public Task Delete(OAuthAccount aggregate, CancellationToken cancellationToken)
    {
        return Task.FromResult(_context.OAuthAccounts.Remove(aggregate));
    }
}
