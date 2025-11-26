using System.Diagnostics.CodeAnalysis;

namespace FluentHttpClient;

/// <summary>
/// Fluent extension methods for creating instances of <see cref="HttpRequestBuilder"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public static class HttpClientExtensions
{
    /// <summary>
    /// Creates a new <see cref="HttpRequestBuilder"/> using the <see cref="HttpClient"/>'s
    /// configured <see cref="HttpClient.BaseAddress"/> as the starting point.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> instance to use for sending requests.</param>
    /// <returns>A new <see cref="HttpRequestBuilder"/> instance initialized with the client's base address.</returns>
    public static HttpRequestBuilder UsingBase(this HttpClient client)
    {
        return new HttpRequestBuilder(client);
    }

    /// <summary>
    /// Creates a new <see cref="HttpRequestBuilder"/> using the specified route
    /// as the initial request URI. The value can be absolute or relative.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> instance to use for sending requests.</param>
    /// <param name="route">The route string for the request URI, which can be absolute or relative.</param>
    /// <returns>A new <see cref="HttpRequestBuilder"/> instance initialized with the specified route.</returns>
    public static HttpRequestBuilder UsingRoute(this HttpClient client, string route)
    {
        return new HttpRequestBuilder(client, route);
    }

    /// <summary>
    /// Creates a new <see cref="HttpRequestBuilder"/> using the specified
    /// <see cref="Uri"/> as the initial request URI.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> instance to use for sending requests.</param>
    /// <param name="uri">The URI for the request.</param>
    /// <returns>A new <see cref="HttpRequestBuilder"/> instance initialized with the specified URI.</returns>
    public static HttpRequestBuilder UsingRoute(this HttpClient client, Uri uri)
    {
        return new HttpRequestBuilder(client, uri);
    }
}
