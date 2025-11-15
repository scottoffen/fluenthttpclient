using System.Net;
using System.Net.Http.Headers;

namespace FluentHttpClient;

/// <summary>
/// Provides a builder pattern for generating and sending an <see cref="HttpRequestMessage"/> 
/// </summary>
public class HttpRequestBuilder
{
    private readonly HttpClient _client;

    /// <summary>
    /// Gets or sets the contents of the HTTP message.
    /// </summary>
    public HttpContent? Content { get; set; }

    /// <summary>
    /// Gets the collection of cookies for the request.
    /// </summary>
    public CookieContainer Cookies { get; } = new CookieContainer();

    /// <summary>
    /// Gets the collection of actions used to configure HTTP request headers.
    /// </summary>
    public List<Action<HttpRequestHeaders>> ConfigureHeaders { get; } = [];

    /// <summary>
    /// Gets or set an action to configure the <see cref="HttpRequestOptions"/>  of the HTTP request.
    /// </summary>
    public Action<HttpRequestOptions>? ConfigureOptionsAction { get; set; }

    /// <summary>
    /// Gets or sets a boolean value to indicate whether content should be buffered (serialized) prior to sending the request.
    /// </summary>
    /// <remarks>For use in rare edge cases; see <a href="https://github.com/dotnet/runtime/issues/30283">this issue</a>.</remarks>
    public bool BufferContentBeforeSending { get; set; }

    /// <summary>
    /// Gets the collection of request query parameters.
    /// </summary>
    /// <remarks>
    /// Use extension methods WithQueryParam() and WithQueryParams() to add values to the collection.
    /// </remarks>
    public List<string> QueryParams { get; } = [];

    /// <summary>
    /// Gets or sets the route used for the HTTP request.
    /// </summary>
    /// <remarks>
    /// Trailing slashes on the route will be removed. If <see cref="HttpClient.BaseAddress"/> has a value, then this value will be appended to it.
    /// </remarks>
    public string? Route { get; private set; }

    /// <summary>
    /// Gets or sets the HTTP message version.
    /// </summary>
    /// <remarks>Defaults to 1.1</remarks>
    public Version Version { get; set; } = HttpVersion.Version1;

    /// <summary>
    /// Gets or sets the policy that determines how <see cref="Version"/> is interpreted and how the final HTTP version is negotiated with the server.
    /// </summary>
    /// <remarks>Defaults to <see cref="HttpVersionPolicy.RequestVersionOrLower"/> </remarks>
    public HttpVersionPolicy VersionPolicy { get; set; } = HttpVersionPolicy.RequestVersionOrLower;

    /// <summary>
    /// Constructs an <see cref="HttpRequestBuilder"/> with the supplied client.
    /// </summary>
    /// <param name="client"></param>
    public HttpRequestBuilder(HttpClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Constructs an <see cref="HttpRequestBuilder"/> with the supplied client and route.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="route"></param>
    public HttpRequestBuilder(HttpClient client, string route) : this(client)
    {
        Route = route;
    }

    /// <summary>
    /// Send an HTTP request as an asynchronous operation.
    /// </summary>
    /// <param name="method"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The request is null or the route is invalid</exception>
    /// <exception cref="InvalidOperationException">The request message was already sent by the HttpClient instance.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="TaskCanceledException">The request failed due to timeout.</exception>
    public Task<HttpResponseMessage> SendAsync(HttpMethod method)
    {
        return SendAsync(method, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
    }

    /// <summary>
    /// Send an HTTP request as an asynchronous operation.
    /// </summary>
    /// <param name="method"></param>
    /// <param name="completionOption"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The request is null or the route is invalid</exception>
    /// <exception cref="InvalidOperationException">The request message was already sent by the HttpClient instance.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="TaskCanceledException">The request failed due to timeout.</exception>
    public Task<HttpResponseMessage> SendAsync(HttpMethod method, HttpCompletionOption completionOption)
    {
        return SendAsync(method, completionOption, CancellationToken.None);
    }

    /// <summary>
    /// Send an HTTP request as an asynchronous operation.
    /// </summary>
    /// <param name="method"></param>
    /// <param name="token"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The request is null or the route is invalid</exception>
    /// <exception cref="InvalidOperationException">The request message was already sent by the HttpClient instance.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="TaskCanceledException">The request failed due to timeout.</exception>
    public Task<HttpResponseMessage> SendAsync(HttpMethod method, CancellationToken token)
    {
        return SendAsync(method, HttpCompletionOption.ResponseContentRead, token);
    }

    /// <summary>
    /// Send an HTTP request as an asynchronous operation.
    /// </summary>
    /// <param name="method"></param>
    /// <param name="completionOption"></param>
    /// <param name="token"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The request is null or the route is invalid</exception>
    /// <exception cref="InvalidOperationException">The request message was already sent by the HttpClient instance.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="TaskCanceledException">The request failed due to timeout.</exception>
    public async Task<HttpResponseMessage> SendAsync(HttpMethod method, HttpCompletionOption completionOption, CancellationToken token)
    {
        if (BufferContentBeforeSending && Content != null)
            await Content.LoadIntoBufferAsync();

        var request = new HttpRequestMessage(method, GenerateRequestUri())
        {
            Content = Content,
            Method = method,
            Version = Version,
            VersionPolicy = VersionPolicy
        };

        ConfigureMessage(request);
        ConfigureOptionsAction?.Invoke(request.Options);

        return await _client
            .SendAsync(request, completionOption, token)
            .ConfigureAwait(false);
    }

    private Uri GenerateRequestUri()
    {
        var baseAddress = _client.BaseAddress?.ToString().TrimEnd('/') ?? string.Empty;
        var route = Route?.TrimStart('/') ?? string.Empty;

        var missingBaseAddress = string.IsNullOrWhiteSpace(baseAddress);
        var missingRoute = string.IsNullOrWhiteSpace(route);

        if (missingBaseAddress && missingRoute)
            throw new ArgumentException("Invalid Route: Client has no base address and no route information was provided");

        if (missingBaseAddress)
        {
            return new Uri($"{route}{QueryParams.ToQueryString()}");
        }

        if (missingRoute)
        {
            return new Uri($"{baseAddress}{QueryParams.ToQueryString()}");
        }

        return new Uri($"{baseAddress}/{route}{QueryParams.ToQueryString()}");
    }

    private void ConfigureMessage(HttpRequestMessage request)
    {
        if (request.RequestUri == null)
            throw new ArgumentException($"{nameof(HttpRequestMessage.RequestUri)} cannot be null");

        if (Content is MultipartContent) request.Headers.ExpectContinue = false;

        ConfigureHeaders.ForEach(x => x.Invoke(request.Headers));

        if (Cookies.Count > 0)
        {
            var cookieHeader = Cookies.GetCookieHeader(request.RequestUri);
            request.Headers.Add("Cookie", cookieHeader);
        }
    }
}
