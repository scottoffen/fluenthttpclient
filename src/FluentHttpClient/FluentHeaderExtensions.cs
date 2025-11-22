namespace FluentHttpClient;

/// <summary>
/// Fluent extension methods for adding headers to an <see cref="HttpRequestBuilder"/> instances.
/// </summary>
public static class FluentHeaderExtensions
{
    private static readonly HashSet<string> _reservedHeaders =
    new(StringComparer.OrdinalIgnoreCase)
    {
        "Host",
        "Content-Length",
        "Transfer-Encoding"
    };

    /// <summary>
    /// Adds the specified header and its value to the request.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
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

        builder.HeaderConfigurators.Add(target =>
            target.TryAddWithoutValidation(key, value));

        return builder;
    }

    /// <summary>
    /// Adds the specified header and its values to the request.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="key"></param>
    /// <param name="values"></param>
    public static HttpRequestBuilder WithHeader(this HttpRequestBuilder builder, string key, IEnumerable<string> values)
    {
        Guard.AgainstNull(key, nameof(key));
        Guard.AgainstNull(values, nameof(values));

        if (_reservedHeaders.Contains(key))
        {
            throw new ArgumentException(
                $"Header '{key}' is managed by HttpClient/HttpContent and cannot be set using FluentHttpClient. " +
                "Configure the request URI or content instead.",
                nameof(key));
        }

        builder.HeaderConfigurators.Add(target =>
            target.TryAddWithoutValidation(key, values));

        return builder;
    }

    /// <summary>
    /// Adds the specified headers and their values to the request.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="headers"></param>
    public static HttpRequestBuilder WithHeaders(this HttpRequestBuilder builder, IEnumerable<KeyValuePair<string, string>> headers)
    {
        Guard.AgainstNull(headers, nameof(headers));

        builder.HeaderConfigurators.Add(target =>
        {
            foreach (var header in headers)
            {
                Guard.AgainstNull(header.Key, "key");
                Guard.AgainstNull(header.Value, "value");

                if (_reservedHeaders.Contains(header.Key))
                {
                    throw new ArgumentException(
                    $"Header '{header.Key}' is managed by HttpClient/HttpContent and cannot be set using FluentHttpClient. " +
                    "Configure the request URI or content instead.",
                    "key");
                }

                target.TryAddWithoutValidation(header.Key, header.Value);
            }
        });

        return builder;
    }

    /// <summary>
    /// Adds the specified headers and their multiple values to the request.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="headers"></param>
    public static HttpRequestBuilder WithHeaders(
        this HttpRequestBuilder builder,
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
    {
        Guard.AgainstNull(headers, nameof(headers));

        builder.HeaderConfigurators.Add(target =>
        {
            foreach (var header in headers)
            {
                Guard.AgainstNull(header.Key, "key");
                Guard.AgainstNull(header.Value, "values");

                if (_reservedHeaders.Contains(header.Key))
                {
                    throw new ArgumentException(
                    $"Header '{header.Key}' is managed by HttpClient/HttpContent and cannot be set using FluentHttpClient. " +
                    "Configure the request URI or content instead.",
                    "key");
                }

                target.TryAddWithoutValidation(header.Key, header.Value);
            }
        });

        return builder;
    }
}
