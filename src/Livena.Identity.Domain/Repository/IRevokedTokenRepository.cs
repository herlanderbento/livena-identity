using Livena.Identity.Domain.Entity;
using Livena.Identity.Domain.Shared;
using Livena.Identity.Domain.Shared.SearchableRepository;

namespace Livena.Identity.Domain.Repository;

public interface IRevokedTokenRepository
    : IRepository<RevokedToken>,
        ISearchableRepository<RevokedToken, string>;
