using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Text;

namespace FluentHttpClient;

/// <summary>
/// Extensions for chaining modifications to the HttpRequestMessage
/// </summary>
public static partial class RestRequestBuilderExtensions
{
    public static RestRequestBuilder UsingRoute(this RestRequestBuilder builder, string route)
    {
        builder.Route = route;
        return builder;
    }

    public static RestRequestBuilder WithAuthentication(this RestRequestBuilder builder, string scheme, string token)
    {
        builder.Request.Headers.Authorization = new AuthenticationHeaderValue(scheme, token);
        return builder;
    }

    public static RestRequestBuilder WithBasicAuthentication(this RestRequestBuilder builder, string username, string password)
    {
        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
        return builder.WithBasicAuthentication(token);
    }

    public static RestRequestBuilder WithBasicAuthentication(this RestRequestBuilder builder, string token)
    {
        return builder.WithAuthentication("Basic", token);
    }

    public static RestRequestBuilder WithContent(this RestRequestBuilder builder, HttpContent content)
    {
        builder.Content = content;
        return builder;
    }

    public static RestRequestBuilder WithContent(this RestRequestBuilder builder, string content)
    {
        builder.Content = new StringContent(content);
        return builder;
    }

    public static RestRequestBuilder WithCookie(this RestRequestBuilder builder, string key, string value)
    {
        builder.Cookies.Add(key, value);
        return builder;
    }

    public static RestRequestBuilder WithCookies(this RestRequestBuilder builder, IEnumerable<KeyValuePair<string, string>> cookies)
    {
        foreach (var cookie in cookies) builder.Cookies.Add(cookie.Key, cookie.Value);
        return builder;
    }

    public static RestRequestBuilder WithHeader(this RestRequestBuilder builder, string key, string value)
    {
        builder.Headers[key] = value;
        return builder;
    }

    public static RestRequestBuilder WithHeaders(this RestRequestBuilder builder, IEnumerable<KeyValuePair<string, string>> headers)
    {
        foreach (var header in headers) builder.Headers[header.Key] = header.Value;
        return builder;
    }

    public static RestRequestBuilder WithOAuthBearerToken(this RestRequestBuilder builder, string token)
    {
        return builder.WithAuthentication("Bearer", token);
    }

    public static RestRequestBuilder WithRequestTimeout(this RestRequestBuilder builder, int seconds)
    {
        return builder.WithRequestTimeout(TimeSpan.FromSeconds(seconds));
    }

    public static RestRequestBuilder WithRequestTimeout(this RestRequestBuilder builder, TimeSpan timeout)
    {
        builder.Timeout = timeout;
        return builder;
    }

    public static RestRequestBuilder WithQueryParam(this RestRequestBuilder builder, string key, string value)
    {
        builder.QueryParams.Add(key, value);
        return builder;
    }

    public static RestRequestBuilder WithQueryParams(this RestRequestBuilder builder, IEnumerable<KeyValuePair<string, string>> queryParams)
    {
        foreach (var param in queryParams) builder.QueryParams.Add(param.Key, param.Value);
        return builder;
    }

    public static RestRequestBuilder WithQueryParams(this RestRequestBuilder builder, NameValueCollection queryParams)
    {
        builder.QueryParams.Add(queryParams);
        return builder;
    }
}

/// <summary>
/// Extensions for sending the HttpRequestMessage via SendAsync
/// </summary>
public static partial class RestRequestBuilderExtensions
{
    public static async Task<HttpResponseMessage> DeleteAsync(this RestRequestBuilder builder, CancellationToken? token = null)
    {
        return await builder.SendAsync(HttpMethod.Delete, token).ConfigureAwait(false);
    }

    public static async Task<HttpResponseMessage> GetAsync(this RestRequestBuilder builder, CancellationToken? token = null)
    {
        return await builder.SendAsync(HttpMethod.Get, token).ConfigureAwait(false);
    }

    public static async Task<HttpResponseMessage> PostAsync(this RestRequestBuilder builder, CancellationToken? token = null)
    {
        return await builder.SendAsync(HttpMethod.Post, token).ConfigureAwait(false);
    }

    public static async Task<HttpResponseMessage> PutAsync(this RestRequestBuilder builder, CancellationToken? token = null)
    {
        return await builder.SendAsync(HttpMethod.Put, token).ConfigureAwait(false);
    }
}