#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif
using System.Net.Http.Headers;

namespace FluentHttpClient;

/// <summary>
/// Fluent extension methods for adding headers to an <see cref="HttpRequestBuilder"/> instances.
/// </summary>
public static class FluentHeaderExtensions
{
#if NET8_0_OR_GREATER
    private static readonly FrozenSet<string> _reservedHeaders =
        FrozenSet.ToFrozenSet(
            ["Host", "Content-Length", "Transfer-Encoding"],
            StringComparer.OrdinalIgnoreCase);
#else
    private static readonly HashSet<string> _reservedHeaders =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "Host",
            "Content-Length",
            "Transfer-Encoding"
        };
#endif

    /// <summary>
    /// Configures HTTP request headers using the strongly-typed <see cref="HttpRequestHeaders"/> API.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use this method when you need to set headers that require strongly-typed values, such as
    /// <c>Authorization</c>, <c>CacheControl</c>, <c>Accept</c>, <c>IfModifiedSince</c>, and other
    /// headers with complex types or specialized formatting.
    /// </para>
    /// <para>
    /// For simple string-based headers, use <see cref="WithHeader(HttpRequestBuilder, string, string)"/>
    /// instead, which provides better performance through direct dictionary storage.
    /// </para>
    /// <para>
    /// Multiple calls to this method will accumulate. Each configuration action is executed when the
    /// request is built, allowing you to compose complex header configurations.
    /// </para>
    /// </remarks>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="configure">An action that configures the strongly-typed request headers.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    /// <example>
    /// Setting cache control headers:
    /// <code>
    /// builder.ConfigureHeaders(headers =>
    /// {
    ///     headers.CacheControl = new CacheControlHeaderValue
    ///     {
    ///         NoCache = true,
    ///         NoStore = true,
    ///         MaxAge = TimeSpan.Zero
    ///     };
    /// });
    /// </code>
    /// 
    /// Setting multiple accept types with quality values:
    /// <code>
    /// builder.ConfigureHeaders(headers =>
    /// {
    ///     headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    ///     headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
    /// });
    /// </code>
    /// 
    /// Setting conditional request headers:
    /// <code>
    /// builder.ConfigureHeaders(headers =>
    /// {
    ///     headers.IfModifiedSince = lastModifiedDate;
    ///     headers.IfNoneMatch.Add(new EntityTagHeaderValue("\"12345\""));
    /// });
    /// </code>
    /// </example>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configure"/> is null.</exception>
    public static HttpRequestBuilder ConfigureHeaders(this HttpRequestBuilder builder, Action<HttpRequestHeaders> configure)
    {
        Guard.AgainstNull(configure, nameof(configure));

        builder.HeaderConfigurators.Add(configure);
        return builder;
    }

    /// <summary>
    /// Adds the specified header and its value to the request.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the same header name is added multiple times, all values are accumulated and sent with the request.
    /// This is useful for headers that accept multiple values, such as <c>Accept</c> or <c>Cache-Control</c>.
    /// </para>
    /// <para>
    /// Header names are case-insensitive per HTTP specifications.
    /// </para>
    /// <para>
    /// Reserved headers (<c>Host</c>, <c>Content-Length</c>, <c>Transfer-Encoding</c>) managed by
    /// <see cref="HttpClient"/> cannot be set and will throw an <see cref="ArgumentException"/> immediately.
    /// </para>
    /// </remarks>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="key">The name of the header to add.</param>
    /// <param name="value">The value of the header.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> or <paramref name="value"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="key"/> is a reserved header.</exception>
    public static HttpRequestBuilder WithHeader(this HttpRequestBuilder builder, string key, string value)
    {
        Guard.AgainstNull(key, nameof(key));
        Guard.AgainstNull(value, nameof(value));

        if (_reservedHeaders.Contains(key))
        {
            throw new ArgumentException(
                $"Header '{key}' is managed by HttpClient/HttpContent and cannot be set using FluentHttpClient. " +
                "Configure the request URI or content instead.",
                nameof(key));
        }

        if (builder.InternalHeaders.TryGetValue(key, out var existingValues))
        {
            var list = existingValues as List<string> ?? existingValues.ToList();
            list.Add(value);
            builder.InternalHeaders[key] = list;
        }
        else
        {
            builder.InternalHeaders[key] = new List<string> { value };
        }

        return builder;
    }

    /// <summary>
    /// Adds the specified header with multiple values to the request.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each value in the collection is added to the header. If the header already exists,
    /// the new values are appended to any existing values. This is useful for headers that
    /// accept multiple values, such as <c>Accept</c> or <c>Cache-Control</c>.
    /// </para>
    /// <para>
    /// This is equivalent to calling <see cref="WithHeader(HttpRequestBuilder, string, string)"/>
    /// multiple times with the same key but different values.
    /// </para>
    /// <para>
    /// Header names are case-insensitive per HTTP specifications.
    /// </para>
    /// <para>
    /// Reserved headers (<c>Host</c>, <c>Content-Length</c>, <c>Transfer-Encoding</c>) managed by
    /// <see cref="HttpClient"/> cannot be set and will throw an <see cref="ArgumentException"/> immediately.
    /// </para>
    /// </remarks>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="key">The name of the header to add.</param>
    /// <param name="values">The collection of values for the header.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> or <paramref name="values"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="key"/> is a reserved header.</exception>
    public static HttpRequestBuilder WithHeader(this HttpRequestBuilder builder, string key, IEnumerable<string> values)
    {
        Guard.AgainstNull(values, nameof(values));

        foreach (var value in values)
        {
            builder.WithHeader(key, value);
        }

        return builder;
    }

    /// <summary>
    /// Adds multiple headers to the request from a collection of key-value pairs.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each header in the collection is validated and added individually. If the same header name
    /// is provided multiple times, all values are accumulated and sent with the request.
    /// This is useful for headers that accept multiple values, such as <c>Accept</c> or <c>Cache-Control</c>.
    /// </para>
    /// <para>
    /// Header names are case-insensitive per HTTP specifications.
    /// </para>
    /// <para>
    /// Reserved headers (<c>Host</c>, <c>Content-Length</c>, <c>Transfer-Encoding</c>) managed by
    /// <see cref="HttpClient"/> cannot be set and will throw an <see cref="ArgumentException"/> immediately.
    /// If any header is invalid or reserved, an exception is thrown immediately before processing
    /// subsequent headers.
    /// </para>
    /// </remarks>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="headers">The collection of headers as key-value pairs to add to the request.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="headers"/> is null, or when any header key or value is null.</exception>
    /// <exception cref="ArgumentException">Thrown when any header key is a reserved header.</exception>
    public static HttpRequestBuilder WithHeaders(this HttpRequestBuilder builder, IEnumerable<KeyValuePair<string, string>> headers)
    {
        Guard.AgainstNull(headers, nameof(headers));

        foreach (var header in headers)
        {
            builder.WithHeader(header.Key, header.Value);
        }

        return builder;
    }

    /// <summary>
    /// Adds multiple headers with potentially multiple values per header to the request.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each header in the collection can have multiple values. All values for a given header
    /// are added to the request. If the same header name is provided multiple times in the collection,
    /// all values are accumulated and sent with the request. This is useful for headers that
    /// accept multiple values, such as <c>Accept</c> or <c>Cache-Control</c>.
    /// </para>
    /// <para>
    /// Header names are case-insensitive per HTTP specifications.
    /// </para>
    /// <para>
    /// Reserved headers (<c>Host</c>, <c>Content-Length</c>, <c>Transfer-Encoding</c>) managed by
    /// <see cref="HttpClient"/> cannot be set and will throw an <see cref="ArgumentException"/> immediately.
    /// If any header is invalid or reserved, an exception is thrown immediately before processing
    /// subsequent headers.
    /// </para>
    /// </remarks>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="headers">The collection of headers where each header can have multiple values.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="headers"/> is null, or when any header key or value collection is null.</exception>
    /// <exception cref="ArgumentException">Thrown when any header key is a reserved header.</exception>
    public static HttpRequestBuilder WithHeaders(
        this HttpRequestBuilder builder,
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
    {
        Guard.AgainstNull(headers, nameof(headers));

        foreach (var header in headers)
        {
            builder.WithHeader(header.Key, header.Value);
        }

        return builder;
    }
}
