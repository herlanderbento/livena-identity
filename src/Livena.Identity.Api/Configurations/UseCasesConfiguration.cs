using FluentValidation;
using Livena.Identity.Application.Events;
using Livena.Identity.Application.Interfaces;
using Livena.Identity.Application.UseCases.User.ChangePassword;
using Livena.Identity.Application.UseCases.User.CreateUser;
using Livena.Identity.Application.UseCases.User.ForgotPassword;
using Livena.Identity.Application.UseCases.User.Logout;
using Livena.Identity.Application.UseCases.User.ResetPassword;
using Livena.Identity.Application.UseCases.User.SendVerificationCode;
using Livena.Identity.Application.UseCases.User.UpdateUser;
using Livena.Identity.Application.UseCases.User.VerifyAccount;
using Livena.Identity.Domain.Repository;
using Livena.Identity.Domain.Shared;
using Livena.Identity.infra.Cryptography;
using Livena.Identity.infra.EntityFramework;
using Livena.Identity.infra.EntityFramework.Repositories;
using Livena.Identity.infra.Keycloak;
using Livena.Identity.Infra.Mail;
using Resend;

namespace Livena.Identity.Api.Configurations;

public static class UseCasesConfiguration
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateUser).Assembly));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Logout).Assembly));
        services.AddRepositories();
        services.AddValidators();
        services.AddDomainEvents();
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IRevokedTokenRepository, RevokedTokenRepository>();
        services.AddTransient<IUserCodeRepository, UserCodeRepository>();
        services.AddTransient<ICryptography, BCryptHasher>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddHttpClient<IKeycloakService, KeycloakService>(
            (serviceProvider, client) =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var baseUrl = configuration["Keycloak:BaseUrl"];
                client.BaseAddress = new Uri(baseUrl!);
            }
        );

        services.AddOptions();
        services.AddHttpClient<ResendClient>();
        services.Configure<ResendClientOptions>(options =>
        {
            var configuration = services
                .BuildServiceProvider()
                .GetRequiredService<IConfiguration>();
            var apiKey =
                configuration["Mail:ResendApiKey"]
                ?? Environment.GetEnvironmentVariable("RESEND_API_KEY");

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException(
                    "Resend API key not configured. Provide Mail:ResendApiKey or RESEND_API_KEY."
                );

            options.ApiToken = apiKey!;
        });
        services.AddTransient<IResend, ResendClient>();
        services.AddScoped<IMailProvider>(sp =>
        {
            var resend = sp.GetRequiredService<IResend>();
            var logger = sp.GetService<ILogger<ResendRazorMailProvider>>();
            var configuration = sp.GetRequiredService<IConfiguration>();
            var from = configuration["Mail:From"] ?? "no-reply@livena.com";

            return new ResendRazorMailProvider(resend, from, logger);
        });

        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateUserInputValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateUserInputValidator>();
        services.AddValidatorsFromAssemblyContaining<LogoutInputValidator>();
        services.AddValidatorsFromAssemblyContaining<VerifyAccountInputValidator>();
        services.AddValidatorsFromAssemblyContaining<SendVerificationCodeInputValidator>();
        services.AddValidatorsFromAssemblyContaining<ForgotPasswordInputValidator>();
        services.AddValidatorsFromAssemblyContaining<ResetPasswordInputValidator>();
        services.AddValidatorsFromAssemblyContaining<ChangePasswordInputValidator>();
        return services;
    }

    private static IServiceCollection AddDomainEvents(this IServiceCollection services)
    {
        services.AddTransient<IDomainEventPublisher, DomainEventPublisher>();

        return services;
    }
}
