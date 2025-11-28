namespace FluentHttpClient;

/// <summary>
/// Fluent extension methods for adding cookies to an <see cref="HttpRequestBuilder"/> instances.
/// </summary>
public static class FluentCookieExtensions
{
    /// <summary>
    /// Adds a single cookie to the request with optional URL encoding of the value.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default, cookie values are URL-encoded using <see cref="Uri.EscapeDataString(System.String)"/> to ensure
    /// special characters (such as <c>;</c>, <c>=</c>, <c>,</c>, and whitespace) do not break the
    /// Cookie header format. This is recommended for most use cases.
    /// </para>
    /// <para>
    /// Set <paramref name="encode"/> to <c>false</c> only if the value is already properly encoded
    /// or if you need to preserve exact byte sequences for legacy systems.
    /// </para>
    /// <para>
    /// Existing cookies with the same name will be overwritten.
    /// </para>
    /// </remarks>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="name">The name of the cookie.</param>
    /// <param name="value">The value of the cookie.</param>
    /// <param name="encode">If <c>true</c> (default), the value will be URL-encoded per RFC 6265. 
    /// Set to <c>false</c> to use the value as-is.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithCookie(
        this HttpRequestBuilder builder,
        string name,
        string value,
        bool encode = true)
    {
        Guard.AgainstNullOrEmpty(name, nameof(name));

        var finalValue = encode
            ? Uri.EscapeDataString(value ?? string.Empty)
            : value ?? string.Empty;

        builder.Cookies[name] = finalValue;
        return builder;
    }

    /// <summary>
    /// Adds multiple cookies to the request.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default, cookie values are URL-encoded using <see cref="Uri.EscapeDataString(System.String)"/> to ensure
    /// special characters (such as <c>;</c>, <c>=</c>, <c>,</c>, and whitespace) do not break the
    /// Cookie header format. This is recommended for most use cases.
    /// </para>
    /// <para>
    /// Set <paramref name="encode"/> to <c>false</c> only if the value is already properly encoded
    /// or if you need to preserve exact byte sequences for legacy systems.
    /// </para>
    /// <para>
    /// Existing cookies with the same name will be overwritten.
    /// </para>
    /// </remarks>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="cookies">The collection of cookies as key-value pairs to add to the request.</param>
    /// <param name="encode">If <c>true</c> (default), the value will be URL-encoded per RFC 6265. 
    /// Set to <c>false</c> to use the value as-is.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithCookies(
        this HttpRequestBuilder builder,
        IEnumerable<KeyValuePair<string, string>> cookies,
        bool encode = true)
    {
        Guard.AgainstNull(cookies, nameof(cookies));

        foreach (var kvp in cookies)
        {
            var key = kvp.Key;
            var value = kvp.Value;

            builder.WithCookie(key, value, encode);
        }

        return builder;
    }
}
