namespace FluentHttpClient;

/// <summary>
/// Fluent extension methods for adding query string parameters to an <see cref="HttpRequestBuilder"/> instance.
/// </summary>
/// <remarks>
/// Query parameters are accumulated on <see cref="HttpRequestBuilder.QueryParameters"/> and applied when the request is built.
/// Multiple values per key are supported; each added value is appended for the same key.
/// </remarks>
public static class FluentQueryParameterExtensions
{
    /// <summary>
    /// Adds a query string parameter with the specified key and value.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithQueryParameter(
        this HttpRequestBuilder builder,
        string key,
        string? value)
    {
        Guard.AgainstNull(key, nameof(key));

        builder.QueryParameters.Add(key, value);
        return builder;
    }

    /// <summary>
    /// Adds a query string parameter with the specified key and a value converted using <see cref="object.ToString"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithQueryParameter(
        this HttpRequestBuilder builder,
        string key,
        object? value)
    {
        Guard.AgainstNull(key, nameof(key));

        builder.QueryParameters.Add(key, value?.ToString());
        return builder;
    }

    /// <summary>
    /// Adds one or more query string parameter values for the specified key.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="key"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithQueryParameter(
        this HttpRequestBuilder builder,
        string key,
        IEnumerable<string?> values)
    {
        Guard.AgainstNull(key, nameof(key));
        Guard.AgainstNull(values, nameof(values));

        builder.QueryParameters.AddRange(key, values);
        return builder;
    }

    /// <summary>
    /// Adds one or more query string parameter values for the specified key, converting each value using <see cref="object.ToString"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="key"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithQueryParameter(
        this HttpRequestBuilder builder,
        string key,
        IEnumerable<object?> values)
    {
        Guard.AgainstNull(key, nameof(key));
        Guard.AgainstNull(values, nameof(values));

        var converted = new List<string?>();

        foreach (var value in values)
        {
            converted.Add(value?.ToString());
        }

        builder.QueryParameters.AddRange(key, converted);
        return builder;
    }

    /// <summary>
    /// Adds multiple query string parameters from the specified sequence of key/value pairs.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithQueryParameters(
        this HttpRequestBuilder builder,
        IEnumerable<KeyValuePair<string, string?>> parameters)
    {
        Guard.AgainstNull(parameters, nameof(parameters));

        foreach (var parameter in parameters)
        {
            builder.QueryParameters.Add(parameter.Key, parameter.Value);
        }

        return builder;
    }

    /// <summary>
    /// Adds multiple query string parameters from the specified sequence of key/value pairs, converting values using <see cref="object.ToString"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithQueryParameters(
        this HttpRequestBuilder builder,
        IEnumerable<KeyValuePair<string, object?>> parameters)
    {
        Guard.AgainstNull(parameters, nameof(parameters));

        foreach (var parameter in parameters)
        {
            builder.QueryParameters.Add(parameter.Key, parameter.Value?.ToString());
        }

        return builder;
    }

    /// <summary>
    /// Adds multiple query string parameters from the specified sequence of keys and value sequences.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithQueryParameters(
        this HttpRequestBuilder builder,
        IEnumerable<KeyValuePair<string, IEnumerable<string?>>> parameters)
    {
        Guard.AgainstNull(parameters, nameof(parameters));

        foreach (var parameter in parameters)
        {
            builder.QueryParameters.AddRange(parameter.Key, parameter.Value);
        }

        return builder;
    }

    /// <summary>
    /// Adds multiple query string parameters from the specified sequence of keys and value sequences, converting values using <see cref="object.ToString"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithQueryParameters(
        this HttpRequestBuilder builder,
        IEnumerable<KeyValuePair<string, IEnumerable<object?>>> parameters)
    {
        Guard.AgainstNull(parameters, nameof(parameters));

        foreach (var parameter in parameters)
        {
            Guard.AgainstNull(parameter.Value, nameof(parameter.Value));

            var converted = new List<string?>();

            foreach (var value in parameter.Value)
            {
                converted.Add(value?.ToString());
            }

            builder.QueryParameters.AddRange(parameter.Key, converted);
        }

        return builder;
    }

    /// <summary>
    /// Adds a query string parameter with the specified key and value when the value is not null.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithQueryParameterIfNotNull(
        this HttpRequestBuilder builder,
        string key,
        string? value)
    {
        Guard.AgainstNull(key, nameof(key));
        return builder.When(value is not null, b => b.WithQueryParameter(key, value));
    }

    /// <summary>
    /// Adds a query string parameter with the specified key and a value converted using <see cref="object.ToString"/> when the value is not null.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithQueryParameterIfNotNull(
        this HttpRequestBuilder builder,
        string key,
        object? value)
    {
        Guard.AgainstNull(key, nameof(key));
        return builder.When(value is not null, b => b.WithQueryParameter(key, value));
    }
}
