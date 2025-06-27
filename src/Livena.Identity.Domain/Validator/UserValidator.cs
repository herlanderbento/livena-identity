using Livena.Identity.Domain.Shared.Validation;

namespace Livena.Identity.Domain.Validator;

public static class UserValidator
{
    public static void Validate(
        string email,
        string password)
    {
        DomainValidation.NotNullOrEmpty(email, nameof(email));
        DomainValidation.MaxLength(email, 255, nameof(email));
        DomainValidation.Email(email, nameof(email));
        
        DomainValidation.NotNullOrEmpty(password, nameof(password));
        DomainValidation.MinLength(password, 6, nameof(password));
        DomainValidation.MaxLength(password, 20, nameof(password));
        
    }
}