namespace FluentHttpClient;

public class HttpRequestBuilder
{
    private readonly HttpClient _client;

    public HttpContent Content
    {
        get { return Request.Content; }
        set { Request.Content = value; }
    }

    public Cookies Cookies { get; } = new Cookies();

    public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();

    public QueryParams QueryParams { get; set; } = new QueryParams();

    public HttpRequestMessage Request { get; } = new HttpRequestMessage();

    public string Route { get; set; }

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
        return await _client.SendAsync(Request, HttpCompletionOption.ResponseContentRead, token.Value);
    }
}