namespace Livena.Identity.Application.Exceptions;

public class WrongCredentialsException : ApplicationException
{
    public WrongCredentialsException(string? message) : base(message)
    {}

    public static void ThrowIfNull(
        object? @object, 
        string exceptionMessage)
    {
        if (@object == null)
            throw new WrongCredentialsException(exceptionMessage);
    }
    
    public static void ThrowIfFalse(
        bool condition, 
        string exceptionMessage)
    {
        if (!condition)
            throw new WrongCredentialsException(exceptionMessage);
    }
}