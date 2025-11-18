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
    /// <param name="builder"></param>
    /// <param name="action"></param>
    public static HttpRequestBuilder ConfigureOptions(this HttpRequestBuilder builder, Action<HttpRequestOptions> action)
    {
        Guard.AgainstNull(action, nameof(action));

        builder.OptionConfigurators.Add(action);
        return builder;
    }

    /// <summary>
    /// Sets a typed option value on the <see cref="HttpRequestMessage.Options"/> collection.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
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
