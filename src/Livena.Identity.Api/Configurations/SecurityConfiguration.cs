using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Livena.Identity.Api.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace Livena.Identity.Api.Configurations;

public static class SecurityConfiguration
{
    public static IServiceCollection AddSecurity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["Jwt:Authority"];
                options.Audience = configuration["Jwt:Audience"];
                options.RequireHttpsMetadata = bool.Parse(configuration["Jwt:RequireHttpsMetadata"] ?? "false");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = bool.Parse(configuration["Jwt:ValidateIssuer"] ?? "true"),
                    ValidateAudience = bool.Parse(configuration["Jwt:ValidateAudience"] ?? "true"),
                    ValidateLifetime = bool.Parse(configuration["Jwt:ValidateLifetime"] ?? "true"),
                    ValidateIssuerSigningKey = bool.Parse(configuration["Jwt:ValidateIssuerSigningKey"] ?? "true"),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
                        if (claimsIdentity != null)
                        {
                            // Extract roles from realm_access.roles
                            var realmAccessClaim = context.Principal?.FindFirst("realm_access");
                            if (realmAccessClaim != null)
                            {
                                try
                                {
                                    var realmAccess = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(realmAccessClaim.Value);
                                    if (realmAccess.TryGetProperty("roles", out var roles))
                                    {
                                        foreach (var role in roles.EnumerateArray())
                                        {
                                            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.GetString()!));
                                        }
                                    }
                                }
                                catch
                                {
                                    // Ignore JSON parsing errors
                                }
                            }
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        services
            .AddAuthorizationBuilder()
            .AddPolicy(Policies.UsersManager, builder => builder.RequireAuthenticatedUser());
        return services;
    }
}