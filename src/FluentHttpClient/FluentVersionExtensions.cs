namespace FluentHttpClient;

/// <summary>
/// Fluent extension methods for configuring the HTTP version and version policy
/// on <see cref="HttpRequestBuilder"/> instances.
/// </summary>
public static class FluentVersionExtensions
{
    /// <summary>
    /// Sets the HTTP message version using a version string such as "1.1" or "2.0".
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="version">The HTTP version as a string (e.g., "1.1", "2.0").</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static TBuilder UsingVersion<TBuilder>(this TBuilder builder, string version)
        where TBuilder : HttpRequestBuilder
    {
        Guard.AgainstNullOrEmpty(version, nameof(version));

        if (!Version.TryParse(version, out var parsed))
        {
            throw new ArgumentException(
                "Version must be a valid version string such as \"1.1\" or \"2.0\".",
                nameof(version));
        }

        builder.Version = parsed;
        return builder;
    }

    /// <summary>
    /// Sets the HTTP message version using the specified major and minor components.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="major">The major version number.</param>
    /// <param name="minor">The minor version number.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static TBuilder UsingVersion<TBuilder>(this TBuilder builder, int major, int minor)
        where TBuilder : HttpRequestBuilder
    {
        builder.Version = new Version(major, minor);
        return builder;
    }

    /// <summary>
    /// Sets the HTTP message version.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="version">The HTTP version to use.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static TBuilder UsingVersion<TBuilder>(this TBuilder builder, Version version)
        where TBuilder : HttpRequestBuilder
    {
        Guard.AgainstNull(version, nameof(version));

        builder.Version = version;
        return builder;
    }
#if NET5_0_OR_GREATER

    /// <summary>
    /// Sets the HTTP message version and the policy that determines how
    /// <see cref="HttpRequestMessage.Version"/> is interpreted and how the final HTTP version
    /// is negotiated with the server.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="version">The HTTP version as a string (e.g., "1.1", "2.0").</param>
    /// <param name="policy">The version policy to use for negotiation.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static TBuilder UsingVersion<TBuilder>(this TBuilder builder, string version, HttpVersionPolicy policy)
        where TBuilder : HttpRequestBuilder
    {
        builder.UsingVersion(version);
        builder.VersionPolicy = policy;
        return builder;
    }

    /// <summary>
    /// Sets the HTTP message version and the policy that determines how
    /// <see cref="HttpRequestMessage.Version"/> is interpreted and how the final HTTP version
    /// is negotiated with the server.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="version">The HTTP version to use.</param>
    /// <param name="policy">The version policy to use for negotiation.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static TBuilder UsingVersion<TBuilder>(this TBuilder builder, Version version, HttpVersionPolicy policy)
        where TBuilder : HttpRequestBuilder
    {
        Guard.AgainstNull(version, nameof(version));

        builder.Version = version;
        builder.VersionPolicy = policy;
        return builder;
    }

    /// <summary>
    /// Sets the HTTP version policy that determines how
    /// <see cref="HttpRequestMessage.Version"/> is interpreted and how the final HTTP version
    /// is negotiated with the server.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="policy">The version policy to use for negotiation.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static TBuilder UsingVersionPolicy<TBuilder>(this TBuilder builder, HttpVersionPolicy policy)
        where TBuilder : HttpRequestBuilder
    {
        builder.VersionPolicy = policy;
        return builder;
    }
#endif
}