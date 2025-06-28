using FluentValidation;
using Livena.Identity.Application.Events;
using Livena.Identity.Application.Interfaces;
using Livena.Identity.Application.UseCases.User.CreateUser;
using Livena.Identity.Domain.Repository;
using Livena.Identity.Domain.Shared;
using Livena.Identity.infra.Cryptography;
using Livena.Identity.infra.EntityFramework;
using Livena.Identity.infra.EntityFramework.Repositories;
using Livena.Identity.infra.Keycloak;

namespace Livena.Identity.Api.Configurations;

public static class UseCasesConfiguration
{
    public static IServiceCollection AddUseCases(
        this IServiceCollection services
    )
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateUser).Assembly));
        services.AddRepositories();
        services.AddValidators();
        services.AddDomainEvents();
        return services;
    }

    private static IServiceCollection AddRepositories(
        this IServiceCollection services
    )
    {
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<ICryptography, BCryptHasher>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddHttpClient<IKeycloakService, KeycloakService>();
        
        return services;
    }
    
    private static IServiceCollection AddValidators(
        this IServiceCollection services
    )
    {
        services.AddValidatorsFromAssemblyContaining<CreateUserInputValidator>();
        return services;
    }
    
    private static IServiceCollection AddDomainEvents(
        this IServiceCollection services)
    {
        services.AddTransient<IDomainEventPublisher, DomainEventPublisher>();

        return services;
    }
    
}

