using Livena.Identity.infra.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Livena.Identity.Api.Configurations;

public static class ConnectionsConfiguration
{
    public static IServiceCollection AddAppConnections(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbConnection(configuration);
        return services;
    }

    private static IServiceCollection AddDbConnection(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("IdentityDb");

        // Configure Npgsql to handle DateTime properly
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", false);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

        services.AddDbContext<LivenaIdentityDbContext>(options =>
            options.UseNpgsql(connectionString)
        );
        return services;
    }

    public static WebApplication MigrateDatabase(this WebApplication app)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (environment == "EndToEndTest")
            return app;
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LivenaIdentityDbContext>();
        dbContext.Database.Migrate();
        return app;
    }
}
