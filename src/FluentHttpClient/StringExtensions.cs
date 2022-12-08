namespace FluentHttpClient;

internal static class StringExtensions
{
    public static bool HasWhiteSpace(this string s)
    {
        if (string.IsNullOrEmpty(s)) return false;
        return s.Any(c => char.IsWhiteSpace(c));
    }

    public static bool Contains(this string s, char[] chars)
    {
        if (string.IsNullOrEmpty(s)) return false;
        return s.Any(c => chars.Contains(c));
    }
}