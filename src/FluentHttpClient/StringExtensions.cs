namespace FluentHttpClient;

internal static class StringExtensions
{
    public static bool HasWhiteSpace(this string s)
    {
        if (s == null) throw new ArgumentNullException("s");

        for (int i = 0; i < s.Length; i++)
        {
            if (char.IsWhiteSpace(s[i])) return true;
        }

        return false;
    }

    public static bool Contains(this string s, char[] chars)
    {
        foreach (var c in chars)
        {
            if (s.Contains<char>(c)) return true;
        }

        return false;
    }
}