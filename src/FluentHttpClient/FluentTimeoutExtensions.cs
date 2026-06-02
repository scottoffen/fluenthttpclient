namespace FluentHttpClient;

/// <summary>
/// Provides extension methods for configuring timeouts in Fluent HTTP Client.
/// </summary>
public static class FluentTimeoutExtensions
{
    internal static readonly string MessageInvalidTimeout = "Timeout must be a positive value.";

    /// <summary>
    /// Clears any per-request timeout that has been set.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static TBuilder ClearTimeout<TBuilder>(this TBuilder builder)
        where TBuilder : HttpRequestBuilder
    {
        builder.Timeout = null;
        return builder;
    }

    /// <summary>
    /// Sets a per-request timeout using the specified number of seconds. Must be positive.
    /// </summary>
    /// <remarks>
    /// The timeout applies only to the current request and determines how long
    /// the client will wait for the request to complete before canceling it.
    /// It does not modify <see cref="HttpClient.Timeout"/> or apply to
    /// other requests made with the same <see cref="HttpClient"/> instance.
    /// </remarks>
    /// <param name="builder"></param>
    /// <param name="seconds"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static TBuilder WithTimeout<TBuilder>(this TBuilder builder, int seconds)
        where TBuilder : HttpRequestBuilder
    {
        if (seconds <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(seconds), MessageInvalidTimeout);
        }

        return builder.WithTimeout(TimeSpan.FromSeconds(seconds));
    }

    /// <summary>
    /// Sets a per-request timeout using the specified <see cref="TimeSpan"/> value. Must be positive.
    /// </summary>
    /// <remarks>
    /// The timeout applies only to the current request and determines how long
    /// the client will wait for the request to complete before canceling it.
    /// It does not modify <see cref="HttpClient.Timeout"/> or apply to
    /// other requests made with the same <see cref="HttpClient"/> instance.
    /// </remarks>
    /// <param name="builder"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static TBuilder WithTimeout<TBuilder>(this TBuilder builder, TimeSpan timeout)
        where TBuilder : HttpRequestBuilder
    {
        Guard.AgainstNull(timeout, nameof(timeout));

        if (timeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout), MessageInvalidTimeout);
        }

        builder.Timeout = timeout;
        return builder;
    }
}