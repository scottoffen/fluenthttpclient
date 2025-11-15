namespace FluentHttpClient;

/// <summary>
/// Fluent extension methods for adding cookies to an <see cref="HttpRequestBuilder"/> instances.
/// </summary>
public static class FluentCookieExtensions
{
    /// <summary>
    /// Adds a single cookie to the request.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithCookie(this HttpRequestBuilder builder, string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cookie name cannot be null or empty.", nameof(name));
        }

        builder.Cookies[name] = value ?? string.Empty;
        return builder;
    }

    /// <summary>
    /// Adds multiple cookies to the request.
    /// </summary>
    /// <remarks>
    /// Existing cookies with the same name will be overwritten.
    /// </remarks>
    /// <param name="builder"></param>
    /// <param name="cookies"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithCookies(
        this HttpRequestBuilder builder,
        IEnumerable<KeyValuePair<string, string>> cookies)
    {
        if (cookies is null)
        {
            throw new ArgumentNullException(nameof(cookies));
        }

        foreach (var (key, value) in cookies)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Cookie name cannot be null or empty.", nameof(cookies));
            }

            builder.Cookies[key] = value ?? string.Empty;
        }

        return builder;
    }
}
