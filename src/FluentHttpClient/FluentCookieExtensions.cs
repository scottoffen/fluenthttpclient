namespace FluentHttpClient;

/// <summary>
/// Fluent extension methods for adding cookies to an <see cref="HttpRequestBuilder"/> instances.
/// </summary>
public static class FluentCookieExtensions
{
    /// <summary>
    /// Adds a single cookie to the request.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="name">The name of the cookie.</param>
    /// <param name="value">The value of the cookie.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
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
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="cookies">The collection of cookies as key-value pairs to add to the request.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithCookies(
        this HttpRequestBuilder builder,
        IEnumerable<KeyValuePair<string, string>> cookies)
    {
        if (cookies is null)
        {
            throw new ArgumentNullException(nameof(cookies));
        }

        foreach (var kvp in cookies)
        {
            var key = kvp.Key;
            var value = kvp.Value;

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Cookie name cannot be null or empty.", nameof(cookies));
            }

            builder.Cookies[key] = value ?? string.Empty;
        }

        return builder;
    }
}
