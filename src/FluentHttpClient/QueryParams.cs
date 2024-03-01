using System.Collections.Specialized;

namespace FluentHttpClient;

/// <summary>
/// Provides access the key/value pairs of the query string of the request
/// </summary>
public class QueryParams : NameValueCollection
{
    public override string ToString()
    {
        if (!HasKeys()) return string.Empty;

        var result = (
            from key in AllKeys
            let value = Get(key)
            where (
                !FluentHttpClientOptions.RemoveEmptyQueryParameters
                || !string.IsNullOrWhiteSpace(value)
            )
            select $"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}"
        ).ToArray();

        return $"?{string.Join("&", result)}";
    }
}