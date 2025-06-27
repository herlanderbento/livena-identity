using Livena.Identity.Domain.Shared.Validation;

namespace Livena.Identity.Domain.Validator;

public static class RevokedTokenValidator
{
    public static void Validate(Guid userId, string accessToken, string refreshToken)
    {
        DomainValidation.NotNullOrEmpty(userId.ToString(), nameof(userId));
        DomainValidation.NotNullOrEmpty(accessToken, nameof(accessToken));
        DomainValidation.NotNullOrEmpty(refreshToken, nameof(refreshToken));
    }
}