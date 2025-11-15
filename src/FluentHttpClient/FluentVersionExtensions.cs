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
    /// <param name="builder"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static HttpRequestBuilder UsingVersion(this HttpRequestBuilder builder, string version)
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            throw new ArgumentException("Version cannot be null or empty.", nameof(version));
        }

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
    /// <param name="builder"></param>
    /// <param name="major"></param>
    /// <param name="minor"></param>
    /// <returns></returns>
    public static HttpRequestBuilder UsingVersion(this HttpRequestBuilder builder, int major, int minor)
    {
        builder.Version = new Version(major, minor);
        return builder;
    }

    /// <summary>
    /// Sets the HTTP message version.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static HttpRequestBuilder UsingVersion(this HttpRequestBuilder builder, Version version)
    {
        if (version is null) throw new ArgumentNullException(nameof(version));

        builder.Version = version;
        return builder;
    }

    /// <summary>
    /// Sets the HTTP message version and the policy that determines how
    /// <see cref="HttpRequestMessage.Version"/> is interpreted and how the final HTTP version
    /// is negotiated with the server.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="version"></param>
    /// <param name="policy"></param>
    /// <returns></returns>
    public static HttpRequestBuilder UsingVersion(this HttpRequestBuilder builder, string version, HttpVersionPolicy policy)
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
    /// <param name="builder"></param>
    /// <param name="version"></param>
    /// <param name="policy"></param>
    /// <returns></returns>
    public static HttpRequestBuilder UsingVersion(this HttpRequestBuilder builder, Version version, HttpVersionPolicy policy)
    {
        if (version is null) throw new ArgumentNullException(nameof(version));

        builder.Version = version;
        builder.VersionPolicy = policy;
        return builder;
    }

    /// <summary>
    /// Sets the HTTP version policy that determines how
    /// <see cref="HttpRequestMessage.Version"/> is interpreted and how the final HTTP version
    /// is negotiated with the server.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="policy"></param>
    /// <returns></returns>
    public static HttpRequestBuilder UsingVersionPolicy(this HttpRequestBuilder builder, HttpVersionPolicy policy)
    {
        builder.VersionPolicy = policy;
        return builder;
    }
}

