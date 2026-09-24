namespace HR_Management_System.Helpers;

public static class DateTimeHelper
{
    private static readonly TimeZoneInfo NigeriaTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById(
            OperatingSystem.IsWindows()
                ? "W. Central Africa Standard Time"
                : "Africa/Lagos"
        );

    public static DateTime Now()
    {
        return TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            NigeriaTimeZone
        );
    }

    public static DateOnly Today()
    {
        return DateOnly.FromDateTime(Now());
    }

    public static TimeOnly CurrentTime()
    {
        return TimeOnly.FromDateTime(Now());
    }
}