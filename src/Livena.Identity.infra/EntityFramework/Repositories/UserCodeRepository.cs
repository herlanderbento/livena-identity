using Livena.Identity.Application.Exceptions;
using Livena.Identity.Domain.Entity;
using Livena.Identity.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Livena.Identity.infra.EntityFramework.Repositories;

public class UserCodeRepository(LivenaIdentityDbContext context) : IUserCodeRepository
{
    private readonly LivenaIdentityDbContext _context = context;
    private DbSet<UserCode> UserCodes => _context.Set<UserCode>();

    public async Task Insert(UserCode aggregate, CancellationToken cancellationToken) =>
        await UserCodes.AddAsync(aggregate, cancellationToken);

    public async Task<UserCode> GetById(Guid id, CancellationToken cancellationToken)
    {
        var model = await UserCodes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        
        return model!;
    }

    public async Task<UserCode> GetByUserId(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var model = await UserCodes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
        return model!;
    }

    public async Task<UserCode> GetByCode(
        string code,
        CancellationToken cancellationToken = default
    )
    {
        var model = await UserCodes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
        return model!;
    }

    public Task Update(UserCode aggregate, CancellationToken _)
    {
        return Task.FromResult(UserCodes.Update(aggregate));
    }

    public Task Delete(UserCode aggregate, CancellationToken _)
    {
        return Task.FromResult(UserCodes.Remove(aggregate));
    }
}
