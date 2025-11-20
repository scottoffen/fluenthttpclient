namespace FluentHttpClient;

/// <summary>
/// Fluent extension methods for adding headers to an <see cref="HttpRequestBuilder"/> instances.
/// </summary>
public static class FluentHeaderExtensions
{
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
                if (header.Key is null)
                {
                    throw new ArgumentException("Header name cannot be null.", nameof(headers));
                }

                if (header.Value is null)
                {
                    throw new ArgumentException(
                        $"Header values for '{header.Key}' cannot be null.",
                        nameof(headers));
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
                if (header.Key is null)
                {
                    throw new ArgumentException("Header name cannot be null.", nameof(headers));
                }

                if (header.Value is null)
                {
                    throw new ArgumentException(
                        $"Header values for '{header.Key}' cannot be null.",
                        nameof(headers));
                }

                target.TryAddWithoutValidation(header.Key, header.Value);
            }
        });

        return builder;
    }
}
