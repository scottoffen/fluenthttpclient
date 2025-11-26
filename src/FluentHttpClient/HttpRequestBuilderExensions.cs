#if NET8_0_OR_GREATER
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
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="method">The HTTP method to use for the request.</param>
    /// <param name="cancellationToken">A cancellation token to observe while building the request.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the built <see cref="HttpRequestMessage"/>.</returns>
    [Experimental("FHCBUILDREQUEST")]
    public static Task<HttpRequestMessage> BuildRequestAsync(
        this HttpRequestBuilder builder,
        HttpMethod method,
        CancellationToken cancellationToken = default)
    {
        return builder.BuildRequest(method, cancellationToken);
    }
}
#endif
