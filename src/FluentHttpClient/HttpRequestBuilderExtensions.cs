using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace FluentHttpClient;

/// <summary>
/// Extensions for setting the properties of the <see cref="HttpRequestBuilder"/> and sending the request.
/// </summary>
public static class HttpRequestBuilderExtensions
{
    /// <summary>
    /// Set the action for <see cref="HttpRequestBuilder.ConfigureOptionsAction"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static HttpRequestBuilder ConfigureOptions(this HttpRequestBuilder builder, Action<HttpRequestOptions> action)
    {
        builder.ConfigureOptionsAction = action;
        return builder;
    }

    /// <summary>
    /// Disables chunked transfer encoding.
    /// </summary>
    /// <param name="builder"></param>
    /// <remarks>
    ///     Chunked transfer encoding is enabled by default; in the overwhelming majority of cases, this is both safe and desireable.
    ///     This disabled chunked transfer encoding by serializing the content, which will auto-populate the Content-Length header.
    ///     See <a href="https://github.com/dotnet/runtime/issues/30283">this issue</a> for a more detailed treatment.
    /// </remarks>
    public static HttpRequestBuilder DisableChunkedTransferEncoding(this HttpRequestBuilder builder)
    {
        builder.TransferEncodingChunked = false;
        return builder;
    }

    /// <summary>
    /// Sets the HTTP message version.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static HttpRequestBuilder UsingVersion(this HttpRequestBuilder builder, string version)
    {
        builder.Version = new Version(version);
        return builder;
    }

    /// <summary>
    /// Sets the HTTP message version.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static HttpRequestBuilder UsingVersion(this HttpRequestBuilder builder, Version version)
    {
        builder.Version = version;
        return builder;
    }

    /// <summary>
    /// Sets the HTTP message version and the policy that determines how <see cref="Version"/> is interpreted and how the final HTTP version is negotiated with the server.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="version"></param>
    /// <param name="policy"></param>
    /// <returns></returns>
    public static HttpRequestBuilder UsingVersion(this HttpRequestBuilder builder, string version, HttpVersionPolicy policy)
    {
        builder.Version = new Version(version);
        builder.VersionPolicy = policy;
        return builder;
    }

    /// <summary>
    /// Sets the HTTP message version and the policy that determines how <see cref="Version"/> is interpreted and how the final HTTP version is negotiated with the server.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="version"></param>
    /// <param name="policy"></param>
    /// <returns></returns>
    public static HttpRequestBuilder UsingVersion(this HttpRequestBuilder builder, Version version, HttpVersionPolicy policy)
    {
        builder.Version = version;
        builder.VersionPolicy = policy;
        return builder;
    }

    /// <summary>
    /// Sets the value of the authentication header for the request.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="scheme"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithAuthentication(this HttpRequestBuilder builder, string scheme, string token)
    {
        builder.ConfigureHeaders.Add(headers =>
        {
            headers.Authorization = new AuthenticationHeaderValue(scheme, token);
        });

        return builder;
    }

    /// <summary>
    /// Sets the value of the authentication header for the request to Basic, using the specified token.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithBasicAuthentication(this HttpRequestBuilder builder, string token)
    {
        return builder.WithAuthentication("Basic", token);
    }

    /// <summary>
    /// Sets the value of the authentication header for the request to Basic using the Base64 encoded username and password as the token.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithBasicAuthentication(this HttpRequestBuilder builder, string username, string password)
    {
        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
        return builder.WithBasicAuthentication(token);
    }

    /// <summary>
    /// Sets the contents of the HTTP message.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithContent(this HttpRequestBuilder builder, HttpContent content)
    {
        builder.Content = content;
        return builder;
    }

    /// <summary>
    /// Sets the contents of the HTTP message.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithContent(this HttpRequestBuilder builder, string content)
    {
        builder.Content = new StringContent(content);
        return builder;
    }

    /// <summary>
    /// Sets the contents of the HTTP message.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="content"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithContent(this HttpRequestBuilder builder, string content, string contentType)
    {
        return builder.WithContent(content, new MediaTypeHeaderValue(contentType));
    }

    /// <summary>
    /// Sets the contents of the HTTP message.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="content"></param>
    /// <param name="contentTypeHeaderValue"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithContent(this HttpRequestBuilder builder, string content, MediaTypeHeaderValue contentTypeHeaderValue)
    {
        builder.Content = new StringContent(content);
        builder.Content.Headers.ContentType = contentTypeHeaderValue;
        return builder;
    }

    /// <summary>
    /// Adds a cookie to the cookie container for this request.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="cookie"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithCookie(this HttpRequestBuilder builder, Cookie cookie)
    {
        builder.Cookies.Add(cookie);
        return builder;
    }

    /// <summary>
    /// Adds a collection of cookies to the cookie container for this request.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="cookieCollection"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithCookie(this HttpRequestBuilder builder, CookieCollection cookieCollection)
    {
        builder.Cookies.Add(cookieCollection);
        return builder;
    }

    /// <summary>
    /// Adds the specified header and its value into the HttpHeaders collection.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithHeader(this HttpRequestBuilder builder, string key, string value)
    {
        builder.ConfigureHeaders.Add((httpHeaders) =>
        {
            httpHeaders.Add(key, value);
        });
        return builder;
    }

    /// <summary>
    /// Adds the specified header and its values into the HttpHeaders collection.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="key"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithHeader(this HttpRequestBuilder builder, string key, IEnumerable<string> values)
    {
        builder.ConfigureHeaders.Add((httpHeaders) =>
        {
            httpHeaders.Add(key, values);
        });
        return builder;
    }

    /// <summary>
    /// Adds the specified headers and their values into the HttpHeaders collection.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="headers"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithHeaders(this HttpRequestBuilder builder, IEnumerable<KeyValuePair<string, string>> headers)
    {
        builder.ConfigureHeaders.Add((httpHeaders) =>
        {
            foreach (var header in headers)
            {
                httpHeaders.Add(header.Key, header.Value);
            }
        });
        return builder;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="JsonContent"/> that contains the specified content serialized to JSON using the default JsonSerializerOptions and assigns it to the Content property of the HttpRequestMessage.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithJsonContent<T>(this HttpRequestBuilder builder, T content)
    {
        return builder.WithJsonContent<T>(content, FluentHttpClientOptions.DefaultJsonSerializerOptions);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="JsonContent"/> that contains the specified content serialized to JSON using the provided JsonSerializerOptions and assigns it to the Content property of the HttpRequestMessage.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="content"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithJsonContent<T>(this HttpRequestBuilder builder, T content, JsonSerializerOptions options)
    {
        builder.Content = JsonContent.Create<T>(content, options: options);
        return builder;
    }

    /// <summary>
    /// Sets the value of the Authentication header for the request.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithOAuthBearerToken(this HttpRequestBuilder builder, string token)
    {
        return builder.WithAuthentication("Bearer", token);
    }

    /// <summary>
    /// Adds the query parameter with the specified key and value to the request Url.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithQueryParam(this HttpRequestBuilder builder, string key, string? value)
    {
        builder.QueryParams.Add(key, (value != null) ? value : string.Empty);
        return builder;
    }

    /// <summary>
    /// Adds the query parameter with the specified key and value to the request Url.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithQueryParam(this HttpRequestBuilder builder, string key, object? value)
    {
        return builder.WithQueryParam(key, (value != null) ? value.ToString() : string.Empty);
    }

    /// <summary>
    /// Adds an enumeration of key value pairs as query parameters to the request Url.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithQueryParams(this HttpRequestBuilder builder, IEnumerable<KeyValuePair<string, string?>> values)
    {
        foreach (var value in values)
        {
            builder.WithQueryParam(value.Key, value.Value);
        }
        return builder;
    }

    /// <summary>
    /// Adds an enumeration of key value pairs as query parameters to the request Url.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithQueryParams(this HttpRequestBuilder builder, IEnumerable<KeyValuePair<string, object?>> values)
    {
        foreach (var value in values)
        {
            builder.WithQueryParam(value.Key, value.Value);
        }
        return builder;
    }

    /// <summary>
    /// Adds the key value pairs from the <see cref="NameValueCollection"/> as query parameters to the request Url.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithQueryParams(this HttpRequestBuilder builder, NameValueCollection values)
    {
        builder.QueryParams.Add(values);
        return builder;
    }

    /// <summary>
    /// Adds the query parameter with the specified key and value to the request Url unless it is null.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithQueryParamIfNotNull(this HttpRequestBuilder builder, string key, string? value)
    {
        if (value != null)
            builder.QueryParams.Add(key, value);
        return builder;
    }

    /// <summary>
    /// Adds the query parameter with the specified key and value to the request Url unless it is null.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithQueryParamIfNotNull(this HttpRequestBuilder builder, string key, object? value)
    {
        if (value != null)
            builder.WithQueryParam(key, value.ToString());
        return builder;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="StringContent"/> that contains the specified content serialized to XML using the <see cref="FluentXmlSerializer.DefaultSettings" /> and assigns it to the Content property of the HttpRequestMessage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithXmlContent<T>(this HttpRequestBuilder builder, T obj)
    {
        return builder.WithXmlContent(obj, FluentXmlSerializer.DefaultSettings.Encoding, FluentXmlSerializer.DefaultContentType);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="StringContent"/> that contains the specified content serialized to XML using the <see cref="FluentXmlSerializer.DefaultSettings" /> and assigns it to the Content property of the HttpRequestMessage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    /// <param name="obj"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithXmlContent<T>(this HttpRequestBuilder builder, T obj, Encoding encoding)
    {
        return builder.WithXmlContent(obj, encoding, FluentXmlSerializer.DefaultContentType);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="StringContent"/> that contains the specified content serialized to XML using the <see cref="FluentXmlSerializer.DefaultSettings" /> and assigns it to the Content property of the HttpRequestMessage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    /// <param name="obj"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithXmlContent<T>(this HttpRequestBuilder builder, T obj, string contentType)
    {
        return builder.WithXmlContent(obj, FluentXmlSerializer.DefaultSettings.Encoding, contentType);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="StringContent"/> that contains the specified content serialized to XML using the <see cref="FluentXmlSerializer.DefaultSettings" /> and assigns it to the Content property of the HttpRequestMessage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    /// <param name="obj"></param>
    /// <param name="encoding"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithXmlContent<T>(this HttpRequestBuilder builder, T obj, Encoding encoding, string contentType)
    {
        var settings = FluentXmlSerializer.DefaultSettings;
        settings.Encoding = encoding;

        var xml = FluentXmlSerializer.Serialize(obj, settings);
        return builder.WithXmlContent(xml, encoding, contentType);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="StringContent"/> that contains the specified content and assigns it to the Content property of the HttpRequestMessage.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="xml"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithXmlContent(this HttpRequestBuilder builder, string xml)
    {
        return builder.WithXmlContent(xml, FluentXmlSerializer.DefaultSettings.Encoding, FluentXmlSerializer.DefaultContentType);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="StringContent"/> that contains the specified content and assigns it to the Content property of the HttpRequestMessage.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="xml"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithXmlContent(this HttpRequestBuilder builder, string xml, Encoding encoding)
    {
        return builder.WithXmlContent(xml, encoding, FluentXmlSerializer.DefaultContentType);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="StringContent"/> that contains the specified content and assigns it to the Content property of the HttpRequestMessage.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="xml"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithXmlContent(this HttpRequestBuilder builder, string xml, string contentType)
    {
        return builder.WithXmlContent(xml, FluentXmlSerializer.DefaultSettings.Encoding, contentType);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="StringContent"/> that contains the specified content and assigns it to the Content property of the HttpRequestMessage.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="xml"></param>
    /// <param name="encoding"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithXmlContent(this HttpRequestBuilder builder, string xml, Encoding encoding, string contentType)
    {
        var content = new StringContent(xml, encoding, contentType);
        content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        builder.Content = content;
        return builder;
    }

    /// <summary>
    /// Send an HTTP DELETE request as an asynchronous operation.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    /// <exception cref="HttpRequestException" />
    /// <exception cref="TaskCanceledException" />
    public static async Task<HttpResponseMessage> DeleteAsync(this HttpRequestBuilder builder)
    {
        return await builder.SendAsync(HttpMethod.Delete).ConfigureAwait(false);
    }

    /// <summary>
    /// Send an HTTP DELETE request as an asynchronous operation.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="completionOption"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    /// <exception cref="HttpRequestException" />
    /// <exception cref="TaskCanceledException" />
    public static async Task<HttpResponseMessage> DeleteAsync(this HttpRequestBuilder builder, HttpCompletionOption completionOption)
    {
        return await builder.SendAsync(HttpMethod.Delete, completionOption).ConfigureAwait(false);
    }

    /// <summary>
    /// Send an HTTP DELETE request as an asynchronous operation.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="token"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    /// <exception cref="HttpRequestException" />
    /// <exception cref="TaskCanceledException" />
    public static async Task<HttpResponseMessage> DeleteAsync(this HttpRequestBuilder builder, CancellationToken token)
    {
        return await builder.SendAsync(HttpMethod.Delete, token).ConfigureAwait(false);
    }

    /// <summary>
    /// Send an HTTP DELETE request as an asynchronous operation.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="completionOption"></param>
    /// <param name="token"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    /// <exception cref="HttpRequestException" />
    /// <exception cref="TaskCanceledException" />
    public static async Task<HttpResponseMessage> DeleteAsync(this HttpRequestBuilder builder, HttpCompletionOption completionOption, CancellationToken token)
    {
        return await builder.SendAsync(HttpMethod.Delete, completionOption, token).ConfigureAwait(false);
    }

    /// <summary>
    /// Send an HTTP GET request as an asynchronous operation.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    /// <exception cref="HttpRequestException" />
    /// <exception cref="TaskCanceledException" />
    public static async Task<HttpResponseMessage> GetAsync(this HttpRequestBuilder builder)
    {
        return await builder.SendAsync(HttpMethod.Get).ConfigureAwait(false);
    }

    /// <summary>
    /// Send an HTTP GET request as an asynchronous operation.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="completionOption"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    /// <exception cref="HttpRequestException" />
    /// <exception cref="TaskCanceledException" />
    public static async Task<HttpResponseMessage> GetAsync(this HttpRequestBuilder builder, HttpCompletionOption completionOption)
    {
        return await builder.SendAsync(HttpMethod.Get, completionOption).ConfigureAwait(false);
    }

    /// <summary>
    /// Send an HTTP GET request as an asynchronous operation.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="token"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    /// <exception cref="HttpRequestException" />
    /// <exception cref="TaskCanceledException" />
    public static async Task<HttpResponseMessage> GetAsync(this HttpRequestBuilder builder, CancellationToken token)
    {
        return await builder.SendAsync(HttpMethod.Get, token).ConfigureAwait(false);
    }

    /// <summary>
    /// Send an HTTP GET request as an asynchronous operation.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="completionOption"></param>
    /// <param name="token"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    /// <exception cref="HttpRequestException" />
    /// <exception cref="TaskCanceledException" />
    public static async Task<HttpResponseMessage> GetAsync(this HttpRequestBuilder builder, HttpCompletionOption completionOption, CancellationToken token)
    {
        return await builder.SendAsync(HttpMethod.Get, completionOption, token).ConfigureAwait(false);
    }

    /// <summary>
    /// Send an HTTP POST request as an asynchronous operation.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    /// <exception cref="HttpRequestException" />
    /// <exception cref="TaskCanceledException" />
    public static async Task<HttpResponseMessage> PostAsync(this HttpRequestBuilder builder)
    {
        return await builder.SendAsync(HttpMethod.Post).ConfigureAwait(false);
    }

    /// <summary>
    /// Send an HTTP POST request as an asynchronous operation.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="completionOption"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    /// <exception cref="HttpRequestException" />
    /// <exception cref="TaskCanceledException" />
    public static async Task<HttpResponseMessage> PostAsync(this HttpRequestBuilder builder, HttpCompletionOption completionOption)
    {
        return await builder.SendAsync(HttpMethod.Post, completionOption).ConfigureAwait(false);
    }

    /// <summary>
    /// Send an HTTP POST request as an asynchronous operation.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="token"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    /// <exception cref="HttpRequestException" />
    /// <exception cref="TaskCanceledException" />
    public static async Task<HttpResponseMessage> PostAsync(this HttpRequestBuilder builder, CancellationToken token)
    {
        return await builder.SendAsync(HttpMethod.Post, token).ConfigureAwait(false);
    }

    /// <summary>
    /// Send an HTTP POST request as an asynchronous operation.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="completionOption"></param>
    /// <param name="token"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    /// <exception cref="HttpRequestException" />
    /// <exception cref="TaskCanceledException" />
    public static async Task<HttpResponseMessage> PostAsync(this HttpRequestBuilder builder, HttpCompletionOption completionOption, CancellationToken token)
    {
        return await builder.SendAsync(HttpMethod.Post, completionOption, token).ConfigureAwait(false);
    }

    /// <summary>
    /// Send an HTTP PUT request as an asynchronous operation.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    /// <exception cref="HttpRequestException" />
    /// <exception cref="TaskCanceledException" />
    public static async Task<HttpResponseMessage> PutAsync(this HttpRequestBuilder builder)
    {
        return await builder.SendAsync(HttpMethod.Put).ConfigureAwait(false);
    }

    /// <summary>
    /// Send an HTTP PUT request as an asynchronous operation.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="completionOption"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    /// <exception cref="HttpRequestException" />
    /// <exception cref="TaskCanceledException" />
    public static async Task<HttpResponseMessage> PutAsync(this HttpRequestBuilder builder, HttpCompletionOption completionOption)
    {
        return await builder.SendAsync(HttpMethod.Put, completionOption).ConfigureAwait(false);
    }

    /// <summary>
    /// Send an HTTP PUT request as an asynchronous operation.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="token"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    /// <exception cref="HttpRequestException" />
    /// <exception cref="TaskCanceledException" />
    public static async Task<HttpResponseMessage> PutAsync(this HttpRequestBuilder builder, CancellationToken token)
    {
        return await builder.SendAsync(HttpMethod.Put, token).ConfigureAwait(false);
    }

    /// <summary>
    /// Send an HTTP PUT request as an asynchronous operation.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="completionOption"></param>
    /// <param name="token"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    /// <exception cref="HttpRequestException" />
    /// <exception cref="TaskCanceledException" />
    public static async Task<HttpResponseMessage> PutAsync(this HttpRequestBuilder builder, HttpCompletionOption completionOption, CancellationToken token)
    {
        return await builder.SendAsync(HttpMethod.Put, completionOption, token).ConfigureAwait(false);
    }
}
