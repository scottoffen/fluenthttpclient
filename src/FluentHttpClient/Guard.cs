namespace FluentHttpClient;

internal static class Guard
{
    public static void AgainstNull(object? value, string? paramName = null)
    {
        if (value is null)
        {
            throw new ArgumentNullException(paramName);
        }
    }
}
