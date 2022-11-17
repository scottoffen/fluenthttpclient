namespace FluentHttpClient;

public class RestRequestBuilder
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

    public string Route { get; set; } = "";

    public TimeSpan Timeout
    {
        get { return _client.Timeout; }
        set { _client.Timeout = value; }
    }

    internal RestRequestBuilder(HttpClient client)
    {
        _client = client;
    }

    public async Task<HttpResponseMessage> SendAsync(HttpMethod method, CancellationToken? token = null)
    {
        Request.Method = method;

        if (Cookies.Any())
            Headers["Cookie"] = (Headers.ContainsKey("Cookie"))
                ? string.Join("; ", new string[] { Headers["Cookies"], Cookies.ToString() })
                : Cookies.ToString();

        foreach (var item in Headers) Request.Headers.Add(item.Key, item.Value);

        if (Request.Content is MultipartContent) Request.Headers.ExpectContinue = false;

        Request.RequestUri = (!string.IsNullOrWhiteSpace(_client.BaseAddress?.ToString()))
            ? new Uri($"{_client.BaseAddress.ToString().TrimEnd('/')}/{Route.TrimStart('/')}{QueryParams}")
            : new Uri($"{Route}{QueryParams}");

        token = token ?? CancellationToken.None;
        return await _client.SendAsync(Request, HttpCompletionOption.ResponseContentRead, token.Value);
    }
}