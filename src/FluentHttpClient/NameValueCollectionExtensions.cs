using System.Collections.Specialized;

namespace FluentHttpClient;

/// <summary>
/// Extension method for converting a <see cref="NameValueCollection"/> into a query string.
/// </summary>
internal static class NameValueCollectionExtensions
{
    /// <summary>
    /// Converts the <see cref="NameValueCollection"/> into a query string.
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static string ToQueryString(this NameValueCollection collection)
    {
        if (!collection.HasKeys()) return string.Empty;

        var result = (
            from key in collection.AllKeys
            let value = collection.Get(key)
            where (
                !FluentHttpClientOptions.RemoveEmptyQueryParameters
                || !string.IsNullOrWhiteSpace(value)
            )
            select $"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}"
        ).ToArray();

        return $"?{string.Join("&", result)}";
    }
}
