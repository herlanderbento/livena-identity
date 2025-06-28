using Livena.Identity.Domain.Entity;
using Livena.Identity.infra.EntityFramework.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Livena.Identity.infra.EntityFramework;

public class LivenaIdentityDbContext: DbContext
{
    public DbSet<User> Users => Set<User>();

    public LivenaIdentityDbContext(DbContextOptions options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new UserConfiguration());
    }
}