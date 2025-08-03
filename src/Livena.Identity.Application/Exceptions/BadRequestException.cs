namespace Livena.Identity.Application.Exceptions;

public class BadRequestException : ApplicationException
{
    public BadRequestException(string? message)
        : base(message) { }

    public static void ThrowIf(bool condition, string exceptionMessage)
    {
        if (condition)
            throw new BadRequestException(exceptionMessage);
    }
}
