namespace Livena.Identity.Domain.Shared;

public static class DateTimeUtils
{
    public static DateTime EnsureUtc(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Utc)
            return dateTime;
        
        if (dateTime.Kind == DateTimeKind.Local)
            return dateTime.ToUniversalTime();
        
        // If Kind is Unspecified, assume it's UTC
        return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }
} 