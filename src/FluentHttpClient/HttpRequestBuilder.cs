namespace FluentHttpClient;

public class HttpRequestBuilder
{
    private readonly HttpClient _client;

    public HttpContent Content
    {
        get { return Request.Content; }
        set { Request.Content = value; }
    }

    /// <summary>
    /// Add or remove request cookies
    /// </summary>
    public Cookies Cookies { get; } = [];

    /// <summary>
    /// Add or remove request headers
    /// </summary>
    public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();

    /// <summary>
    /// Get or set an exception handler for exceptions of type <see cref="HttpRequestException"/>
    /// </summary>
    public Action<HttpRequestException> HttpRequestExceptionHandler { get; set; }

    /// <summary>
    /// Get or set request query parameters
    /// </summary>
    public QueryParams QueryParams { get; set; } = [];

    /// <summary>
    /// The request object
    /// </summary>
    public HttpRequestMessage Request { get; } = new HttpRequestMessage();

    /// <summary>
    /// The request route
    /// </summary>
    /// <remarks>If the HttpClient has a base address, this value will be appended to the end of the base address, and extra slashes removed.</remarks>
    public string Route { get; set; }

    /// <summary>
    /// Gets or sets the timespan to wait before the request times out
    /// </summary>
    public TimeSpan Timeout
    {
        get { return _client.Timeout; }
        set { _client.Timeout = value; }
    }

    internal HttpRequestBuilder(HttpClient client)
    {
        _client = client;
    }

    public async Task<HttpResponseMessage> SendAsync(HttpMethod method, CancellationToken? token = null)
    {
        var noBaseAddress = string.IsNullOrWhiteSpace(_client.BaseAddress?.ToString());
        var noRoute = string.IsNullOrWhiteSpace(Route);

        if (noBaseAddress && noRoute)
            throw new ArgumentException("Invalid Route: Client has not base address and no route information was provided");

        Request.Method = method;

        if (Cookies.Any())
            Headers["Cookie"] = (Headers.ContainsKey("Cookie"))
                ? string.Join("; ", new string[] { Headers["Cookies"], Cookies.ToString() })
                : Cookies.ToString();

        foreach (var item in Headers) Request.Headers.Add(item.Key, item.Value);

        if (Request.Content is MultipartContent) Request.Headers.ExpectContinue = false;

        if (noRoute)
        {
            Request.RequestUri = new Uri($"{_client.BaseAddress}{QueryParams}");
        }
        else
        {
            Request.RequestUri = noBaseAddress
                ? new Uri($"{Route}{QueryParams}")
                : new Uri($"{_client.BaseAddress.ToString().TrimEnd('/')}/{Route.TrimStart('/')}{QueryParams}");
        }

        token ??= CancellationToken.None;

        try
        {
            return await _client.SendAsync(Request, HttpCompletionOption.ResponseContentRead, token.Value);
        }
        catch (HttpRequestException hre)
        {
            if (HttpRequestExceptionHandler != null)
            {
                HttpRequestExceptionHandler(hre);
                return new NullHttpResponseMessage();
            }
            else
            {
                throw;
            }
        }
    }
}