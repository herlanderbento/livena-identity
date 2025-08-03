using Livena.Identity.Domain.Entity;
using Livena.Identity.Domain.Shared;

namespace Livena.Identity.Domain.Repository;

public interface IUserCodeRepository : IRepository<UserCode>
{
    public Task<UserCode> GetByUserId(Guid userId, CancellationToken cancellationToken = default);
    public Task<UserCode> GetByCode(string code, CancellationToken cancellationToken = default);
}
