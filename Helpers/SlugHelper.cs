namespace HR_Management_System.Helpers;

public static class SlugHelper
{
    public static string Slugify(string value)
        => string.Join(
            "-",
            value.Trim()
                .ToLowerInvariant()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
        );
}