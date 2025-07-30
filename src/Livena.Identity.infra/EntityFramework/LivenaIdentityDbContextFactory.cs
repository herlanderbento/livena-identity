using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Livena.Identity.infra.EntityFramework;

public class LivenaIdentityDbContextFactory : IDesignTimeDbContextFactory<LivenaIdentityDbContext>
{
    public LivenaIdentityDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LivenaIdentityDbContext>();

        var basePath = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), "../Livena.Identity.Api")
        );

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString = configuration.GetConnectionString("IdentityDb");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new Exception("Connection string is null or empty!");

        optionsBuilder.UseNpgsql(connectionString);

        return new LivenaIdentityDbContext(optionsBuilder.Options);
    }
}
