namespace FluentHttpClient;

public static class HttpClientExtensions
{
    /// <summary>
    /// Allows setting a route for the HTTP request and initiates the chaining of extensions
    /// </summary>
    /// <param name="client">HttpClient instance to be used for request</param>
    /// <param name="route">Endpoint URL, can be empty if the base address already targets the endpoint</param>
    /// <returns></returns>
    public static HttpRequestBuilder UsingRoute(this HttpClient client, string route)
    {
        return new HttpRequestBuilder(client).UsingRoute(route);
    }

    /// <summary>
    /// Returns an <see cref="HttpRequestBuilder"/> with an empty route
    /// </summary>
    /// <param name="client">HttpClient instance to be used for request</param>
    /// <remarks>The request will be sent to the BaseUrl of the <see cref="HttpClient"/> </remarks>
    public static HttpRequestBuilder WithoutRoute(this HttpClient client)
    {
        return new HttpRequestBuilder(client).UsingRoute(string.Empty);
    }
}