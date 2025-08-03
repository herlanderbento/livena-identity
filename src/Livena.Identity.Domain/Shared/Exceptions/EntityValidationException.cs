using Livena.Identity.Domain.Shared.Validation;

namespace Livena.Identity.Domain.Shared.Exceptions;

public class EntityValidationException : Exception
{
    public IReadOnlyCollection<ValidationError>? Errors { get; }

    public EntityValidationException(
        string? message,
        IReadOnlyCollection<ValidationError>? errors = null
    )
        : base(message) => Errors = errors;
}
