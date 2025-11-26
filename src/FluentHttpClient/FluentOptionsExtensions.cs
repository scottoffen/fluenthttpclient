#if NET5_0_OR_GREATER
namespace FluentHttpClient;

/// <summary>
/// Fluent extension methods for configuring the <see cref="HttpRequestOptions"/>  on an <see cref="HttpRequestBuilder"/> instances.
/// </summary>
public static class FluentOptionsExtensions
{
    /// <summary>
    /// Adds a configurator for modifying the <see cref="HttpRequestMessage.Options"/> collection.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="action">The action to configure the request options.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder ConfigureOptions(this HttpRequestBuilder builder, Action<HttpRequestOptions> action)
    {
        Guard.AgainstNull(action, nameof(action));

        builder.OptionConfigurators.Add(action);
        return builder;
    }

    /// <summary>
    /// Sets a typed option value on the <see cref="HttpRequestMessage.Options"/> collection.
    /// </summary>
    /// <typeparam name="T">The type of the option value.</typeparam>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="key">The key identifying the option to set.</param>
    /// <param name="value">The value to set for the option.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithOption<T>(this HttpRequestBuilder builder, HttpRequestOptionsKey<T> key, T value)
    {
        builder.OptionConfigurators.Add(options =>
        {
            options.Set(key, value);
        });

        return builder;
    }
}
#endif
