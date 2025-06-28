using Livena.Identity.Domain.Shared.Validation;

namespace Livena.Identity.Domain.Validator;

public static class UserValidator
{
    public static void Validate(
        string username,
        string? email,
        string password)
    {
        DomainValidation.NotNullOrEmpty(username, nameof(username));
        DomainValidation.MinLength(username, 6, nameof(username));
        DomainValidation.MaxLength(username, 255, nameof(username));
        
        DomainValidation.MaxLength(email, 255, nameof(email));
        DomainValidation.Email(email, nameof(email));
        
        DomainValidation.NotNullOrEmpty(password, nameof(password));
        DomainValidation.MinLength(password, 6, nameof(password));
        DomainValidation.MaxLength(password, 20, nameof(password));
        
    }
}