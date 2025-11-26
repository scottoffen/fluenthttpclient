using System.Net;
using System.Net.Http.Headers;

namespace FluentHttpClient;

/// <summary>
/// Provides a builder pattern for generating and sending an <see cref="HttpRequestMessage"/>
/// using a configured <see cref="HttpClient"/>.
/// </summary>
public class HttpRequestBuilder
{
    internal static readonly string MessageInvalidBaseAddress = "HttpClient.BaseAddress must not contain a query string or fragment.";
    internal static readonly string MessageInvalidRoute = "Route must not contain a query string or fragment. Use QueryParameters to specify query values.";
    internal static readonly string MessageEmptyRoute = "Missing or invalid route provided to constructor.";
    internal static readonly string MessageMissingRoute = "Client has no base address and no route information was provided.";
    internal static readonly string CookieHeaderName = "Cookie";

    private readonly HttpClient _client;
    private readonly Uri? _route;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpRequestBuilder"/> class
    /// with the specified <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="client">The HTTP client to use for sending requests.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> is null.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="client"/> has a <see cref="HttpClient.BaseAddress"/> that
    /// contains a query string or fragment.
    /// </exception>
    internal HttpRequestBuilder(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));

        if (_client.BaseAddress is not null)
        {
            if (!string.IsNullOrEmpty(_client.BaseAddress.Query) ||
                !string.IsNullOrEmpty(_client.BaseAddress.Fragment))
            {
                throw new ArgumentException(MessageInvalidBaseAddress, nameof(client));
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpRequestBuilder"/> class
    /// with the specified <see cref="HttpClient"/> and route.
    /// </summary>
    /// <param name="client">The HTTP client to use for sending requests.</param>
    /// <param name="route">The route string for the request.</param>
    internal HttpRequestBuilder(HttpClient client, string route)
        : this(client, CreateRouteUri(route))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpRequestBuilder"/> class
    /// with the specified <see cref="HttpClient"/> and route.
    /// </summary>
    /// <param name="client">The HTTP client to use for sending requests.</param>
    /// <param name="route">The route URI for the request.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="route"/> is null.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="route"/> contains a query string or fragment.
    /// </exception>
    internal HttpRequestBuilder(HttpClient client, Uri route) : this(client)
    {
        Guard.AgainstNull(route, nameof(route));

        if (route.IsAbsoluteUri)
        {
            if (!string.IsNullOrEmpty(route.Query) || !string.IsNullOrEmpty(route.Fragment))
            {
                throw new ArgumentException(MessageInvalidRoute, nameof(route));
            }
        }
        else
        {
            var text = route.OriginalString;
            if (text.IndexOfAny(['?', '#']) >= 0)
            {
                throw new ArgumentException(MessageInvalidRoute, nameof(route));
            }
        }

        _route = route;
    }

    /// <summary>
    /// Gets or sets the contents of the HTTP message.
    /// </summary>
    public HttpContent? Content { get; set; }

    /// <summary>
    /// Returns the cookies that will be sent on this HTTP request.
    /// </summary>
    /// <remarks>
    /// These cookies are serialized into the Cookie header for this request only.
    /// Long-lived cookie management should be configured on the underlying handler used by <see cref="HttpClient"/>.
    /// Values are not automatically encoded when sending; encoding should occur prior to adding the cookie.
    /// </remarks>
    public IDictionary<string, string> Cookies { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal);

    /// <summary>
    /// Returns the collection of deferred configurators that are executed during
    /// final request construction.
    /// </summary>
    /// <remarks>
    /// Deferred configurators allow conditional or context-dependent configuration
    /// to be applied when the <see cref="HttpRequestMessage"/> is actually built,
    /// rather than when the fluent calls are made.
    /// This is useful for scenarios where the condition depends on late-bound
    /// state (such as ambient context values, feature flags, or other runtime
    /// information available only when the request is being created).
    /// </remarks>
    public List<Action<HttpRequestBuilder>> DeferredConfigurators { get; } = [];

    /// <summary>
    /// Returns the collection of actions used to configure HTTP request headers.
    /// </summary>
    public List<Action<HttpRequestHeaders>> HeaderConfigurators { get; } = [];

#if NET5_0_OR_GREATER
    /// <summary>
    /// Returns the collection of actions used to configure the <see cref="HttpRequestOptions"/> of the HTTP request.
    /// </summary>
    public List<Action<HttpRequestOptions>> OptionConfigurators { get; } = [];

#endif
    /// <summary>
    /// Gets or sets a value that indicates whether content should be buffered
    /// (fully serialized into memory) prior to sending the request.
    /// </summary>
    /// <remarks>
    /// This is intended for rare edge cases where buffering is required.
    /// When enabled, large payloads may have a significant memory impact.
    /// See https://github.com/dotnet/runtime/issues/30283 for additional context.
    /// </remarks>
    public bool BufferRequestContent { get; set; }

    /// <summary>
    /// Returns the collection of query string parameters for this request.
    /// </summary>
    /// <remarks>
    /// All query string values should be added through this collection.
    /// <see cref="HttpClient.BaseAddress"/> and <see cref="Route"/> must not contain query components.
    /// </remarks>
    public HttpQueryParameterCollection QueryParameters { get; } = [];

    /// <summary>
    /// Returns the route used for the HTTP request.
    /// </summary>
    public string? Route => _route?.OriginalString;

    /// <summary>
    /// Gets or sets the HTTP message version.
    /// </summary>
    /// <remarks>Defaults to HTTP/1.1.</remarks>
    public Version Version { get; set; } = HttpVersion.Version11;

#if NET5_0_OR_GREATER
    /// <summary>
    /// Gets or sets the policy that determines how <see cref="Version"/> is interpreted
    /// and how the final HTTP version is negotiated with the server.
    /// </summary>
    /// <remarks>Defaults to <see cref="HttpVersionPolicy.RequestVersionOrLower"/>.</remarks>
    public HttpVersionPolicy VersionPolicy { get; set; } = HttpVersionPolicy.RequestVersionOrLower;

#endif
    /// <summary>
    /// Sends an HTTP request as an asynchronous operation.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="HttpCompletionOption.ResponseContentRead"/> and a default cancellation token.
    /// </remarks>
    /// <param name="method">The HTTP method as a string (e.g., "GET", "POST").</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="method"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the request URI cannot be constructed because neither <see cref="HttpClient.BaseAddress"/>
    /// nor <see cref="Route"/> is specified.
    /// </exception>
    public Task<HttpResponseMessage> SendAsync(string method)
        => SendAsync(method, HttpCompletionOption.ResponseContentRead, CancellationToken.None);

    /// <summary>
    /// Sends an HTTP request as an asynchronous operation.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="HttpCompletionOption.ResponseContentRead"/> and a default cancellation token.
    /// </remarks>
    /// <param name="method">The HTTP method to use for the request.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="method"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the request URI cannot be constructed because neither <see cref="HttpClient.BaseAddress"/>
    /// nor <see cref="Route"/> is specified.
    /// </exception>
    public Task<HttpResponseMessage> SendAsync(HttpMethod method)
        => SendAsync(method, HttpCompletionOption.ResponseContentRead, CancellationToken.None);

    /// <summary>
    /// Sends an HTTP request as an asynchronous operation.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="HttpCompletionOption.ResponseContentRead"/>.
    /// </remarks>
    /// <param name="method">The HTTP method as a string (e.g., "GET", "POST").</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="method"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the request URI cannot be constructed because neither <see cref="HttpClient.BaseAddress"/>
    /// nor <see cref="Route"/> is specified.
    /// </exception>
    public Task<HttpResponseMessage> SendAsync(string method, CancellationToken cancellationToken)
        => SendAsync(method, HttpCompletionOption.ResponseContentRead, cancellationToken);

    /// <summary>
    /// Sends an HTTP request as an asynchronous operation.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="HttpCompletionOption.ResponseContentRead"/>.
    /// </remarks>
    /// <param name="method">The HTTP method to use for the request.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="method"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the request URI cannot be constructed because neither <see cref="HttpClient.BaseAddress"/>
    /// nor <see cref="Route"/> is specified.
    /// </exception>
    public Task<HttpResponseMessage> SendAsync(HttpMethod method, CancellationToken cancellationToken)
        => SendAsync(method, HttpCompletionOption.ResponseContentRead, cancellationToken);

    /// <summary>
    /// Sends an HTTP request as an asynchronous operation.
    /// </summary>
    /// <remarks>
    /// Uses the specified <see cref="HttpCompletionOption"/> and a default cancellation token.
    /// </remarks>
    /// <param name="method">The HTTP method as a string (e.g., "GET", "POST").</param>
    /// <param name="completionOption">Indicates when the operation should complete.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="method"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the request URI cannot be constructed because neither <see cref="HttpClient.BaseAddress"/>
    /// nor <see cref="Route"/> is specified.
    /// </exception>
    public Task<HttpResponseMessage> SendAsync(string method, HttpCompletionOption completionOption)
        => SendAsync(method, completionOption, CancellationToken.None);

    /// <summary>
    /// Sends an HTTP request as an asynchronous operation.
    /// </summary>
    /// <remarks>
    /// Uses the specified <see cref="HttpCompletionOption"/> and a default cancellation token.
    /// </remarks>
    /// <param name="method">The HTTP method to use for the request.</param>
    /// <param name="completionOption">Indicates when the operation should complete.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="method"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the request URI cannot be constructed because neither <see cref="HttpClient.BaseAddress"/>
    /// nor <see cref="Route"/> is specified.
    /// </exception>
    public Task<HttpResponseMessage> SendAsync(HttpMethod method, HttpCompletionOption completionOption)
        => SendAsync(method, completionOption, CancellationToken.None);

    /// <summary>
    /// Sends an HTTP request as an asynchronous operation using the configured builder state.
    /// </summary>
    /// <param name="method">The HTTP method as a string (e.g., "GET", "POST").</param>
    /// <param name="completionOption">Indicates when the operation should complete.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="method"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the request URI cannot be constructed because neither <see cref="HttpClient.BaseAddress"/>
    /// nor <see cref="Route"/> is specified.
    /// </exception>
    public Task<HttpResponseMessage> SendAsync(
        string method,
        HttpCompletionOption completionOption,
        CancellationToken cancellationToken)
    {
        Guard.AgainstNullOrEmpty(method, nameof(method));
        return SendAsync(new HttpMethod(method.ToUpper()), completionOption, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP request as an asynchronous operation using the configured builder state.
    /// </summary>
    /// <param name="method">The HTTP method to use for the request.</param>
    /// <param name="completionOption">Indicates when the operation should complete.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="method"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the request URI cannot be constructed because neither <see cref="HttpClient.BaseAddress"/>
    /// nor <see cref="Route"/> is specified.
    /// </exception>
    public async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        HttpCompletionOption completionOption,
        CancellationToken cancellationToken)
    {
        using var request = await BuildRequest(method, cancellationToken).ConfigureAwait(false);

        return await _client
            .SendAsync(request, completionOption, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Builds and configures the <see cref="HttpRequestMessage"/> for this builder instance.
    /// Intended for internal and testing use. Use <see cref="SendAsync(HttpMethod)"/> and its overloads to send requests.
    /// </summary>
    /// <param name="method">The HTTP method to use for the request.</param>
    /// <param name="cancellationToken">A cancellation token to observe while building the request.</param>
    /// <returns>The configured <see cref="HttpRequestMessage"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="method"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the request URI cannot be constructed because neither <see cref="HttpClient.BaseAddress"/>
    /// nor <see cref="Route"/> is specified.
    /// </exception>
    internal async Task<HttpRequestMessage> BuildRequest(HttpMethod method, CancellationToken cancellationToken)
    {
        Guard.AgainstNull(method, nameof(method));

        foreach (var configure in DeferredConfigurators)
        {
            configure(this);
        }

        if (BufferRequestContent && Content != null)
        {
#if NET9_0_OR_GREATER
            await Content.LoadIntoBufferAsync(cancellationToken).ConfigureAwait(false);
#else
            await Content.LoadIntoBufferAsync().ConfigureAwait(false);
#endif
        }

        var uri = BuildRequestUri();

        var request = new HttpRequestMessage(method, uri)
        {
            Content = Content,
            Version = Version,
#if NET5_0_OR_GREATER
            VersionPolicy = VersionPolicy
#endif
        };

        ApplyConfiguration(request);

        return request;
    }

    /// <summary>
    /// Builds the final request URI from <see cref="HttpClient.BaseAddress"/>,
    /// <see cref="Route"/>, and <see cref="QueryParameters"/>.
    /// </summary>
    /// <returns>The constructed <see cref="Uri"/>.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when both <see cref="HttpClient.BaseAddress"/> and <see cref="Route"/> are missing.
    /// </exception>
    internal Uri BuildRequestUri()
    {
        if (_client.BaseAddress is null && _route is null)
        {
            throw new ArgumentException(MessageMissingRoute, nameof(Route));
        }

        var queryString = QueryParameters.ToQueryString();

        if (_route is null)
        {
            return new Uri(queryString, UriKind.Relative);
        }

        if (_route.IsAbsoluteUri)
        {
            var path = _route.GetLeftPart(UriPartial.Path);
            return new Uri($"{path}{queryString}", UriKind.Absolute);
        }

        var routeText = _route.OriginalString.Trim();
        return new Uri($"{routeText}{queryString}", UriKind.RelativeOrAbsolute);
    }

    /// <summary>
    /// Applies headers, cookies, and options from the builder to the request.
    /// </summary>
    /// <param name="request">The HTTP request message to configure.</param>
    private void ApplyConfiguration(HttpRequestMessage request)
    {
        if (Content is MultipartContent)
        {
            request.Headers.ExpectContinue = false;
        }

        for (var i = 0; i < HeaderConfigurators.Count; i++)
        {
            HeaderConfigurators[i](request.Headers);
        }

        if (Cookies.Count > 0)
        {
            var cookieHeader = string.Join(
                "; ",
                Cookies.Select(kvp => $"{kvp.Key}={kvp.Value}"));

            if (!string.IsNullOrEmpty(cookieHeader))
            {
                request.Headers.Add(CookieHeaderName, cookieHeader);
            }
        }
#if NET5_0_OR_GREATER

        for (var i = 0; i < OptionConfigurators.Count; i++)
        {
            OptionConfigurators[i](request.Options);
        }
#endif
    }

    /// <summary>
    /// Creates a <see cref="Uri"/> instance from the specified route string,
    /// trimming whitespace and validating that the value is not null or empty.
    /// </summary>
    /// <remarks>
    /// This method should only be used by the <see cref="HttpRequestBuilder"/> constructors.
    /// </remarks>
    internal static Uri CreateRouteUri(string route)
    {
        Guard.AgainstNull(route, nameof(route));

        var trimmed = route.Trim();

        if (trimmed.Length == 0)
        {
            throw new ArgumentException(MessageEmptyRoute, nameof(route));
        }

        try
        {
            return new Uri(trimmed, UriKind.RelativeOrAbsolute);
        }
        catch (UriFormatException ex)
        {
            throw new ArgumentException(MessageEmptyRoute, nameof(route), ex);
        }
    }
}
