#if NET5_0_OR_GREATER
namespace FluentHttpClient;

/// <summary>
/// Fluent extension methods for configuring the <see cref="HttpRequestOptions"/>  on an TBuilder instances.
/// </summary>
public static class FluentOptionsExtensions
{
    /// <summary>
    /// Adds a configurator for modifying the <see cref="HttpRequestMessage.Options"/> collection.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder, which must inherit from <see cref="HttpRequestBuilder"/>.</typeparam>
    /// <param name="builder">The TBuilder instance.</param>
    /// <param name="action">The action to configure the request options.</param>
    /// <returns>The TBuilder for method chaining.</returns>
    public static TBuilder ConfigureOptions<TBuilder>(this TBuilder builder, Action<HttpRequestOptions> action)
        where TBuilder : HttpRequestBuilder
    {
        Guard.AgainstNull(action, nameof(action));

        builder.OptionConfigurators.Add(action);
        return builder;
    }

    /// <summary>
    /// Sets a typed option value on the <see cref="HttpRequestMessage.Options"/> collection.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder, which must inherit from <see cref="HttpRequestBuilder"/>.</typeparam>
    /// <typeparam name="T">The type of the option value.</typeparam>
    /// <param name="builder">The TBuilder instance.</param>
    /// <param name="key">The key identifying the option to set.</param>
    /// <param name="value">The value to set for the option.</param>
    /// <returns>The TBuilder for method chaining.</returns>
    public static TBuilder WithOption<TBuilder, T>(this TBuilder builder, HttpRequestOptionsKey<T> key, T value)
        where TBuilder : HttpRequestBuilder
    {
        builder.OptionConfigurators.Add(options =>
        {
            options.Set(key, value);
        });

        return builder;
    }
}
#endif