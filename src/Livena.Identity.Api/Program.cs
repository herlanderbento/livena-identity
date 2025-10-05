using DotNetEnv;
using Livena.Identity.Api.Configurations;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder
    .Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Migrations.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("IdentityDb");
if (!string.IsNullOrEmpty(connectionString))
{
    connectionString = connectionString
        .Replace("#DB_HOST#", Environment.GetEnvironmentVariable("DB_HOST"))
        .Replace("#DB_PORT#", Environment.GetEnvironmentVariable("DB_PORT"))
        .Replace("#DB_NAME#", Environment.GetEnvironmentVariable("DB_NAME"))
        .Replace("#DB_USER#", Environment.GetEnvironmentVariable("DB_USER"))
        .Replace("#DB_PASSWORD#", Environment.GetEnvironmentVariable("DB_PASSWORD"));

    configuration["ConnectionStrings:IdentityDb"] = connectionString;
}

configuration["Keycloak:Realm"] = Environment.GetEnvironmentVariable("KEYCLOAK_REALM");
configuration["Keycloak:ClientId"] = Environment.GetEnvironmentVariable("KEYCLOAK_CLIENT_ID");
configuration["Keycloak:ClientSecret"] = Environment.GetEnvironmentVariable(
    "KEYCLOAK_CLIENT_SECRET"
);
configuration["Keycloak:TokenUrl"] = Environment.GetEnvironmentVariable("KEYCLOAK_TOKEN_URL");
configuration["Keycloak:BaseUrl"] = Environment.GetEnvironmentVariable("KEYCLOAK_BASE_URL");

configuration["Jwt:Authority"] = Environment.GetEnvironmentVariable("JWT_AUTHORITY");
configuration["Jwt:Audience"] = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

configuration["Mail:From"] = Environment.GetEnvironmentVariable("MAIL_FROM");
configuration["Mail:ResendApiKey"] = Environment.GetEnvironmentVariable("RESEND_API_KEY");

builder
    .Services.AddAppConnections(builder.Configuration)
    .AddUseCases()
    .AddSecurity(builder.Configuration)
    .AddAndConfigureControllers()
    .AddHttpLogging(logging =>
    {
        logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
        logging.RequestBodyLogLimit = 4096;
        logging.ResponseBodyLogLimit = 4096;
    })
    .AddCors(p =>
        p.AddPolicy(
            "CORS",
            corsPolicyBuilder =>
            {
                corsPolicyBuilder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
            }
        )
    );

var app = builder.Build();

app.UseHttpLogging();
app.UseDocumentation();
app.UseCors("CORS");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MigrateDatabase();
app.Run();

public partial class Program { }
