using System.Collections.Specialized;

namespace FluentHttpClient;

/// <summary>
/// Provides access the key/value pairs of the query string of the request
/// </summary>
public class QueryParams : NameValueCollection
{
    public override string ToString()
    {
        return HasKeys()
            ? string.Empty
            : "?" + string.Join("&", (from key in AllKeys let value = Get(key) select Uri.EscapeDataString(key) + "=" + Uri.EscapeDataString(value)).ToArray());
    }
}