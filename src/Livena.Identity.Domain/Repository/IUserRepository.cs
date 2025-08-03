using Livena.Identity.Domain.Entity;
using Livena.Identity.Domain.Shared;
using Livena.Identity.Domain.Shared.SearchableRepository;

namespace Livena.Identity.Domain.Repository;

public interface IUserRepository : IRepository<User>, ISearchableRepository<User, string>
{
    public Task<User> GetByUsername(string username, CancellationToken cancellationToken = default);
    public Task<User> GetByEmail(string email, CancellationToken cancellationToken = default);
    public Task<User> GetByPhone(string phone, CancellationToken cancellationToken = default);
    public Task<IReadOnlyList<User>> GetListByIds(
        List<Guid> ids,
        CancellationToken cancellationToken
    );
}
