using Livena.Identity.Domain.Entity;
using Livena.Identity.Domain.Shared;

namespace Livena.Identity.Domain.Repository;

public interface IOAuthAccountRepository : IRepository<OAuthAccount>
{
    public Task<OAuthAccount?> GetByUserIdAndProvider(
        Guid userId,
        string provider,
        CancellationToken cancellationToken
    );
    public Task<OAuthAccount?> GetByProviderAndProviderUserId(
        string provider,
        string providerUserId,
        CancellationToken cancellationToken
    );
}
