namespace FluentHttpClient;

// TODO: Support more complex cookies

/// <summary>
/// Provides access the cookies for the request
/// </summary>
public class Cookies : Dictionary<string, string>
{
    private static readonly char[] _invalidNameChars = @"()<>@,;:\/[]?={}""".ToCharArray();
    private static readonly char[] _invalidValueChars = @",;\""".ToCharArray();

    public new string this[string key]
    {
        get { return base[key]; }
        set { base[ValidateName(key)] = ValidateValue(value); }
    }

    public new void Add(string key, string value)
    {
        base.Add(ValidateName(key), ValidateValue(value));
    }

    public new bool TryAdd(string key, string value)
    {
        return base.TryAdd(ValidateName(key), ValidateValue(value));
    }

    public override string ToString()
    {
        return Count <= 0
            ? string.Empty
            : string.Join("; ", (from key in Keys let value = base[key] select $"{key}={Uri.EscapeDataString(value)}").ToArray());
    }

    private static string ValidateName(string name)
    {
        if (name.HasWhiteSpace() || name.Contains(_invalidNameChars))
            throw new ArgumentOutOfRangeException($"Invalid cookie name {name}");

        return name;
    }

    private static string ValidateValue(string value)
    {
        if (value.HasWhiteSpace() || value.Contains(_invalidValueChars))
            throw new ArgumentOutOfRangeException($"Invalid cookie value {value}");

        return value;
    }
}
