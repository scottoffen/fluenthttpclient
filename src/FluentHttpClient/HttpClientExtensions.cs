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
    /// <remarks>
    /// This method does not accept or set a route, and this library gives you no public way to add one
    /// afterward. Because of that, the resulting request always targets <see cref="HttpClient.BaseAddress"/>
    /// as-is, so the trailing slash requirement described on <c>UsingRoute</c> does not apply here. It only
    /// matters for derived builder types that introduce their own route, since they inherit the same
    /// <see cref="Uri"/> combination behavior.
    /// </remarks>
    public static HttpRequestBuilder UsingBase(this HttpClient client)
    {
        return new HttpRequestBuilder(client);
    }

    /// <summary>
    /// Creates a new TBuilder using the <see cref="HttpClient"/>'s
    /// configured <see cref="HttpClient.BaseAddress"/> as the starting point.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the request builder to create.</typeparam>
    /// <param name="client">The <see cref="HttpClient"/> instance to use for sending requests.</param>
    /// <returns>A new instance of <typeparamref name="TBuilder"/> initialized with the client's base address.</returns>
    /// <remarks>
    /// This method does not accept or set a route, and this library gives you no public way to add one
    /// afterward. Because of that, the resulting request always targets <see cref="HttpClient.BaseAddress"/>
    /// as-is, so the trailing slash requirement described on <c>UsingRoute</c> does not apply here. It only
    /// matters for derived builder types that introduce their own route, since they inherit the same
    /// <see cref="Uri"/> combination behavior.
    /// </remarks>
#if NET7_0_OR_GREATER
    [RequiresDynamicCode("Constructs TBuilder with Activator.CreateInstance, which can require runtime code generation. For Native AOT, use the UsingRoute overload that takes a factory delegate.")]    
#endif
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("Constructs TBuilder with Activator.CreateInstance, which is not trimming-safe. For trimmed or AOT apps, use the UsingRoute overload that takes a factory delegate.")]
#endif
    public static TBuilder UsingBase<TBuilder>(this HttpClient client)
        where TBuilder : HttpRequestBuilder
    {
        return BuilderFactory<TBuilder>.Create(client);
    }

    /// <summary>
    /// Creates a new <see cref="HttpRequestBuilder"/> using the specified route
    /// as the initial request URI. The value can be absolute or relative.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> instance to use for sending requests.</param>
    /// <param name="route">The route string for the request URI, which can be absolute or relative.</param>
    /// <returns>A new <see cref="HttpRequestBuilder"/> instance initialized with the specified route.</returns>
    /// <remarks>
    /// When the route is relative, <see cref="HttpClient.BaseAddress"/> must end with a trailing slash
    /// for the route to be appended to it correctly. This is standard <see cref="Uri"/> combination
    /// behavior (RFC 3986 relative reference resolution), not something this library controls. For
    /// example, a base address of <c>https://example.com/api/posts</c> combined with the route <c>1</c>
    /// resolves to <c>https://example.com/api/1</c>, dropping the <c>posts</c> segment, because the base
    /// address has no trailing slash. Use <c>https://example.com/api/posts/</c> instead to get
    /// <c>https://example.com/api/posts/1</c>. Adding a leading slash to the route does not fix this
    /// either. A route of <c>/1</c> is treated as an absolute path and replaces the entire base address
    /// path, giving you <c>https://example.com/1</c> instead, dropping <c>api</c> and <c>posts</c> both.
    /// </remarks>
    public static HttpRequestBuilder UsingRoute(this HttpClient client, string route)
    {
        return new HttpRequestBuilder(client, route);
    }

    /// <summary>
    /// Creates a new TBuilder using the specified route
    /// as the initial request URI. The value can be absolute or relative.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the request builder to create.</typeparam>
    /// <param name="client">The <see cref="HttpClient"/> instance to use for sending requests.</param>
    /// <param name="route">The route string for the request URI, which can be absolute or relative.</param>
    /// <returns>A new instance of <typeparamref name="TBuilder"/> initialized with the specified route.</returns>
    /// <remarks>
    /// When the route is relative, <see cref="HttpClient.BaseAddress"/> must end with a trailing slash
    /// for the route to be appended to it correctly. This is standard <see cref="Uri"/> combination
    /// behavior (RFC 3986 relative reference resolution), not something this library controls. For
    /// example, a base address of <c>https://example.com/api/posts</c> combined with the route <c>1</c>
    /// resolves to <c>https://example.com/api/1</c>, dropping the <c>posts</c> segment, because the base
    /// address has no trailing slash. Use <c>https://example.com/api/posts/</c> instead to get
    /// <c>https://example.com/api/posts/1</c>. Adding a leading slash to the route does not fix this
    /// either. A route of <c>/1</c> is treated as an absolute path and replaces the entire base address
    /// path, giving you <c>https://example.com/1</c> instead, dropping <c>api</c> and <c>posts</c> both.
    /// </remarks>
#if NET7_0_OR_GREATER
    [RequiresDynamicCode("Constructs TBuilder with Activator.CreateInstance, which can require runtime code generation. For Native AOT, use the UsingRoute overload that takes a factory delegate.")]    
#endif
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("Constructs TBuilder with Activator.CreateInstance, which is not trimming-safe. For trimmed or AOT apps, use the UsingRoute overload that takes a factory delegate.")]
#endif
    public static TBuilder UsingRoute<TBuilder>(this HttpClient client, string route)
        where TBuilder : HttpRequestBuilder
    {
        return BuilderFactory<TBuilder>.Create(client, route);
    }

    /// <summary>
    /// Creates a new <see cref="HttpRequestBuilder"/> using the specified
    /// <see cref="Uri"/> as the initial request URI.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> instance to use for sending requests.</param>
    /// <param name="uri">The URI for the request.</param>
    /// <returns>A new <see cref="HttpRequestBuilder"/> instance initialized with the specified URI.</returns>
    /// <remarks>
    /// When the route is relative, <see cref="HttpClient.BaseAddress"/> must end with a trailing slash
    /// for the route to be appended to it correctly. This is standard <see cref="Uri"/> combination
    /// behavior (RFC 3986 relative reference resolution), not something this library controls. For
    /// example, a base address of <c>https://example.com/api/posts</c> combined with the route <c>1</c>
    /// resolves to <c>https://example.com/api/1</c>, dropping the <c>posts</c> segment, because the base
    /// address has no trailing slash. Use <c>https://example.com/api/posts/</c> instead to get
    /// <c>https://example.com/api/posts/1</c>. Adding a leading slash to the route does not fix this
    /// either. A route of <c>/1</c> is treated as an absolute path and replaces the entire base address
    /// path, giving you <c>https://example.com/1</c> instead, dropping <c>api</c> and <c>posts</c> both.
    /// </remarks>
    public static HttpRequestBuilder UsingRoute(this HttpClient client, Uri uri)
    {
        return new HttpRequestBuilder(client, uri);
    }

    /// <summary>
    /// Creates a new TBuilder using the specified <see cref="Uri"/> as the initial request URI.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the request builder to create.</typeparam>
    /// <param name="client">The <see cref="HttpClient"/> instance to use for sending requests.</param>
    /// <param name="uri">The URI for the request.</param>
    /// <returns>A new instance of <typeparamref name="TBuilder"/> initialized with the specified URI.</returns>
    /// <remarks>
    /// When the route is relative, <see cref="HttpClient.BaseAddress"/> must end with a trailing slash
    /// for the route to be appended to it correctly. This is standard <see cref="Uri"/> combination
    /// behavior (RFC 3986 relative reference resolution), not something this library controls. For
    /// example, a base address of <c>https://example.com/api/posts</c> combined with the route <c>1</c>
    /// resolves to <c>https://example.com/api/1</c>, dropping the <c>posts</c> segment, because the base
    /// address has no trailing slash. Use <c>https://example.com/api/posts/</c> instead to get
    /// <c>https://example.com/api/posts/1</c>. Adding a leading slash to the route does not fix this
    /// either. A route of <c>/1</c> is treated as an absolute path and replaces the entire base address
    /// path, giving you <c>https://example.com/1</c> instead, dropping <c>api</c> and <c>posts</c> both.
    /// </remarks>
#if NET7_0_OR_GREATER
    [RequiresDynamicCode("Constructs TBuilder with Activator.CreateInstance, which can require runtime code generation. For Native AOT, use the UsingRoute overload that takes a factory delegate.")]    
#endif
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("Constructs TBuilder with Activator.CreateInstance, which is not trimming-safe. For trimmed or AOT apps, use the UsingRoute overload that takes a factory delegate.")]
#endif
    public static TBuilder UsingRoute<TBuilder>(this HttpClient client, Uri uri)
        where TBuilder : HttpRequestBuilder
    {
        return BuilderFactory<TBuilder>.Create(client, uri);
    }
}
