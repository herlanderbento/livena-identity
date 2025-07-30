using Livena.Identity.Api.Filters;
using Livena.Identity.Api.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

namespace Livena.Identity.Api.Configurations;

public static class ControllersConfiguration
{
    public static IServiceCollection AddAndConfigureControllers(this IServiceCollection services)
    {
        services.AddControllers(options => options.Filters.Add(typeof(ApiGlobalExceptionFilter)));

        services.Configure<RouteOptions>(options =>
        {
            options.LowercaseUrls = true;
        });

        services.AddScoped<RequestValidator>();

        services.AddDocumentation();
        return services;
    }

    private static IServiceCollection AddDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                "v1",
                new OpenApiInfo { Title = "Livena Identity Api", Version = "v1" }
            );

            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Enter a valid token (optional for public routes)",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer",
                }
            );

            options.OperationFilter<AuthOperationFilter>();
        });

        return services;
    }

    public static WebApplication UseDocumentation(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Livena Identity Api V1");
            options.RoutePrefix = string.Empty;
        });
        return app;
    }
}
