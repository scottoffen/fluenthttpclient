using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace FluentHttpClient;

/// <summary>
/// Extensions for chaining modifications to the HttpRequestMessage
/// </summary>
public static partial class HttpRequestBuilderExtensions
{
    public static HttpRequestBuilder UsingRoute(this HttpRequestBuilder builder, string route)
    {
        builder.Route = route;
        return builder;
    }

    public static HttpRequestBuilder WithAuthentication(this HttpRequestBuilder builder, string scheme, string token)
    {
        builder.Request.Headers.Authorization = new AuthenticationHeaderValue(scheme, token);
        return builder;
    }

    public static HttpRequestBuilder WithBasicAuthentication(this HttpRequestBuilder builder, string username, string password)
    {
        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
        return builder.WithBasicAuthentication(token);
    }

    public static HttpRequestBuilder WithBasicAuthentication(this HttpRequestBuilder builder, string token)
    {
        return builder.WithAuthentication("Basic", token);
    }

    public static HttpRequestBuilder WithContent(this HttpRequestBuilder builder, HttpContent content)
    {
        builder.Content = content;
        return builder;
    }

    public static HttpRequestBuilder WithContent(this HttpRequestBuilder builder, string content)
    {
        builder.Content = new StringContent(content);
        return builder;
    }

    public static HttpRequestBuilder WithCookie(this HttpRequestBuilder builder, string key, string value)
    {
        builder.Cookies.Add(key, value);
        return builder;
    }

    public static HttpRequestBuilder WithCookies(this HttpRequestBuilder builder, IEnumerable<KeyValuePair<string, string>> cookies)
    {
        foreach (var cookie in cookies) builder.Cookies.Add(cookie.Key, cookie.Value);
        return builder;
    }

    public static HttpRequestBuilder WithHeader(this HttpRequestBuilder builder, string key, string value)
    {
        builder.Headers[key] = value;
        return builder;
    }

    public static HttpRequestBuilder WithHeaders(this HttpRequestBuilder builder, IEnumerable<KeyValuePair<string, string>> headers)
    {
        foreach (var header in headers) builder.Headers[header.Key] = header.Value;
        return builder;
    }

    public static HttpRequestBuilder WithOAuthBearerToken(this HttpRequestBuilder builder, string token)
    {
        return builder.WithAuthentication("Bearer", token);
    }

    public static HttpRequestBuilder WithRequestTimeout(this HttpRequestBuilder builder, int seconds)
    {
        return builder.WithRequestTimeout(TimeSpan.FromSeconds(seconds));
    }

    public static HttpRequestBuilder WithRequestTimeout(this HttpRequestBuilder builder, TimeSpan timeout)
    {
        builder.Timeout = timeout;
        return builder;
    }

    public static HttpRequestBuilder WithQueryParam(this HttpRequestBuilder builder, string key, string value)
    {
        builder.QueryParams.Add(key, value);
        return builder;
    }

    public static HttpRequestBuilder WithQueryParams(this HttpRequestBuilder builder, IEnumerable<KeyValuePair<string, string>> queryParams)
    {
        foreach (var param in queryParams) builder.QueryParams.Add(param.Key, param.Value);
        return builder;
    }

    public static HttpRequestBuilder WithQueryParams(this HttpRequestBuilder builder, NameValueCollection queryParams)
    {
        builder.QueryParams.Add(queryParams);
        return builder;
    }
}

/// <summary>
/// Extensions for sending the HttpRequestMessage via SendAsync
/// </summary>
public static partial class HttpRequestBuilderExtensions
{
    public static async Task<HttpResponseMessage> DeleteAsync(this HttpRequestBuilder builder, CancellationToken? token = null)
    {
        return await builder.SendAsync(HttpMethod.Delete, token).ConfigureAwait(false);
    }

    public static async Task<HttpResponseMessage> GetAsync(this HttpRequestBuilder builder, CancellationToken? token = null)
    {
        return await builder.SendAsync(HttpMethod.Get, token).ConfigureAwait(false);
    }

    public static async Task<HttpResponseMessage> PostAsync(this HttpRequestBuilder builder, CancellationToken? token = null)
    {
        return await builder.SendAsync(HttpMethod.Post, token).ConfigureAwait(false);
    }

    public static async Task<HttpResponseMessage> PutAsync(this HttpRequestBuilder builder, CancellationToken? token = null)
    {
        return await builder.SendAsync(HttpMethod.Put, token).ConfigureAwait(false);
    }
}

/// <summary>
/// Extensions for adding json content
/// </summary>
public static partial class HttpRequestBuilderExtensions
{
    public static HttpRequestBuilder WithJsonContent(this HttpRequestBuilder builder, object content)
    {
        return builder.WithJsonContent(content, FluentHttpClient.DefaultJsonSerializerOptions);
    }

    public static HttpRequestBuilder WithJsonContent(this HttpRequestBuilder builder, object content, JsonSerializerOptions options)
    {
        builder.Content = JsonContent.Create(content, options: options);
        return builder;
    }
}