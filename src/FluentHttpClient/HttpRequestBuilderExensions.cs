using System.Diagnostics.CodeAnalysis;

namespace FluentHttpClient;

/// <summary>
/// Exposes an experimental interface.
/// </summary>
[ExcludeFromCodeCoverage]
public static class HttpRequestBuilderExtensions
{
    /// <summary>
    /// Builds an <see cref="HttpRequestMessage"/> using the current state of the builder.
    /// This API is experimental and may change in future versions.
    /// </summary>
    [Experimental("FHCBUILDREQUEST")]
    public static Task<HttpRequestMessage> BuildRequestAsync(
        this HttpRequestBuilder builder,
        HttpMethod method,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.BuildRequest(method, cancellationToken);
    }
}
