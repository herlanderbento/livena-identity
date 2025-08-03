using Livena.Identity.Domain.Entity;
using Livena.Identity.Domain.Shared;
using Livena.Identity.infra.EntityFramework.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Globalization;

namespace Livena.Identity.infra.EntityFramework;

public class LivenaIdentityDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RevokedToken> RevokedTokens => Set<RevokedToken>();
    public DbSet<UserCode> UserCodes => Set<UserCode>();

    public LivenaIdentityDbContext(DbContextOptions options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RevokedTokenConfiguration());
        modelBuilder.ApplyConfiguration(new UserCodeConfiguration());

        modelBuilder.Ignore<DomainEvent>();

        ApplySnakeCaseNames(modelBuilder);
    }

    private static void ApplySnakeCaseNames(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(ToSnakeCase(entity.GetTableName()));

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.Name));
            }

            foreach (var key in entity.GetKeys())
            {
                key.SetName(ToSnakeCase(key.GetName()));
            }

            foreach (var index in entity.GetIndexes())
            {
                index.SetDatabaseName(ToSnakeCase(index.GetDatabaseName()));
            }

            foreach (var fk in entity.GetForeignKeys())
            {
                fk.SetConstraintName(ToSnakeCase(fk.GetConstraintName()));
            }
        }
    }

    private static string ToSnakeCase(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input ?? string.Empty;

        var result = new List<char>();
        for (int i = 0; i < input.Length; i++)
        {
            var c = input[i];
            if (char.IsUpper(c))
            {
                if (i > 0 && input[i - 1] != '_')
                    result.Add('_');

                result.Add(char.ToLower(c, CultureInfo.InvariantCulture));
            }
            else
            {
                result.Add(c);
            }
        }

        return new string(result.ToArray());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        NormalizeDateTimes();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        NormalizeDateTimes();
        return base.SaveChanges();
    }

    private void NormalizeDateTimes()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            foreach (var property in entry.Properties)
            {
                if (property.Metadata.ClrType == typeof(DateTime) && property.CurrentValue != null)
                {
                    var dateTime = (DateTime)property.CurrentValue;
                    if (dateTime.Kind == DateTimeKind.Unspecified)
                    {
                        property.CurrentValue = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                    }
                    else if (dateTime.Kind == DateTimeKind.Local)
                    {
                        property.CurrentValue = dateTime.ToUniversalTime();
                    }
                }
            }
        }
    }
}
