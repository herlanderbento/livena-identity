using Livena.Identity.Domain.Entity;
using Livena.Identity.Domain.Repository;
using Livena.Identity.infra.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Livena.Identity.infra.EntityFramework.Repositories;

public class OAuthAccountRepository : IOAuthAccountRepository
{
    private readonly LivenaIdentityDbContext _context;

    public OAuthAccountRepository(LivenaIdentityDbContext context)
    {
        _context = context;
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

    public async Task Insert(OAuthAccount oAuthAccount, CancellationToken cancellationToken)
    {
        await _context.OAuthAccounts.AddAsync(oAuthAccount, cancellationToken);
    }

    public async Task Update(OAuthAccount oAuthAccount, CancellationToken cancellationToken)
    {
        _context.OAuthAccounts.Update(oAuthAccount);
        await Task.CompletedTask;
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
        var oAuthAccount = await _context.OAuthAccounts.FindAsync([id], cancellationToken);
        if (oAuthAccount != null)
        {
            _context.OAuthAccounts.Remove(oAuthAccount);
        }
    }
}
