using System.Net.Mail;
using Livena.Identity.Domain.Shared.Exceptions;

namespace Livena.Identity.Domain.Shared.Validation;

public class DomainValidation
{
    public static void NotNull(object? target, string fieldName)
    {
        if (target is null)
            throw new EntityValidationException($"{fieldName} should not be null");
    }
    
    public static void NotNullOrEmpty(string? target, string fieldName)
    {
        if (String.IsNullOrWhiteSpace(target))
            throw new EntityValidationException(
                $"{fieldName} should not be empty or null");
    }

    public static void MinLength(string target, int minLength, string fieldName)
    {
        if (target.Length < minLength)
            throw new EntityValidationException($"{fieldName} should be at least {minLength} characters long");
    }
    public static void MaxLength(string target, int maxLength, string fieldName)
    {
        if (target.Length > maxLength)
            throw new EntityValidationException($"{fieldName} should be less or equal {maxLength} characters long");
    }
    
    public static void Email(string target, string fieldName)
    {
        try
        {
            var email = new MailAddress(target);
            if (email.Address != target)
                throw new EntityValidationException($"{fieldName} is not a valid email address");
        }
        catch
        {
            throw new EntityValidationException($"{fieldName} is not a valid email address");
        }
    }
}