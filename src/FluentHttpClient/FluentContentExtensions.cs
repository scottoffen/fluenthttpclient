using System.Net.Http.Headers;
using System.Text;

namespace FluentHttpClient;

/// <summary>
/// Fluent extension methods for adding content to the <see cref="HttpRequestBuilder"/>.
/// </summary>
public static class FluentContentExtensions
{
    /// <summary>
    /// Enables request content buffering for this request.
    /// </summary>
    /// <remarks>
    /// <para>When enabled, the request content is fully serialized into memory
    /// before the request is sent. This is useful for rare scenarios where
    /// the underlying <see cref="HttpClient"/> handler requires the content
    /// length to be known in advance, or when streaming content may cause
    /// protocol or middleware issues.</para>
    ///
    /// <para>Buffering can have a significant memory impact for large payloads.</para>
    /// </remarks>
    /// <param name="builder"></param>
    public static HttpRequestBuilder WithBufferedContent(this HttpRequestBuilder builder)
    {
        builder.BufferRequestContent = true;
        return builder;
    }

    /// <summary>
    /// Sets the request content using an existing <see cref="HttpContent"/> instance.
    /// </summary>
    /// <remarks>
    /// Use this for adding any pre-built content that inherits from <see cref="HttpContent"/> (e.g. <see cref="MultipartContent"/>).
    /// </remarks>
    public static HttpRequestBuilder WithContent(
        this HttpRequestBuilder builder,
        HttpContent content)
    {
        builder.Content = content;
        return builder;
    }

    /// <summary>
    /// Sets the request content using form URL encoded data represented by a dictionary.
    /// </summary>
    public static HttpRequestBuilder WithFormContent(
        this HttpRequestBuilder builder,
        Dictionary<string, string> data)
    {
#if NET5_0
        var pairs = data.Select(static kvp =>
            new KeyValuePair<string?, string?>(kvp.Key, kvp.Value));

        builder.Content = new FormUrlEncodedContent(pairs);
#else
        builder.Content = new FormUrlEncodedContent(data);
#endif
        return builder;
    }

    /// <summary>
    /// Sets the request content using form URL encoded data represented by a sequence
    /// of key/value pairs. Allows multiple values for the same key.
    /// </summary>
    public static HttpRequestBuilder WithFormContent(
        this HttpRequestBuilder builder,
        IEnumerable<KeyValuePair<string, string>> data)
    {
#if NET5_0
        var pairs = data.Select(static kvp =>
            new KeyValuePair<string?, string?>(kvp.Key, kvp.Value));

        builder.Content = new FormUrlEncodedContent(pairs);
#else
        builder.Content = new FormUrlEncodedContent(data);
#endif
        return builder;
    }

    #region String Content

    /// <summary>
    /// Sets the request content using a <see cref="StringContent"/> with default encoding.
    /// </summary>
    public static HttpRequestBuilder WithContent(
        this HttpRequestBuilder builder,
        string content)
    {
        builder.Content = new StringContent(content);
        return builder;
    }

    /// <summary>
    /// Sets the request content using a <see cref="StringContent"/> created with the specified encoding.
    /// </summary>
    public static HttpRequestBuilder WithContent(
        this HttpRequestBuilder builder,
        string content,
        Encoding encoding)
    {
        builder.Content = new StringContent(content, encoding);
        return builder;
    }

    /// <summary>
    /// Sets the request content using a <see cref="StringContent"/> with UTF-8 encoding and the specified media type.
    /// </summary>
    public static HttpRequestBuilder WithContent(
        this HttpRequestBuilder builder,
        string content,
        string mediaType)
    {
        builder.Content = new StringContent(content, Encoding.UTF8, mediaType);
        return builder;
    }

    /// <summary>
    /// Sets the request content using a <see cref="StringContent"/> with the specified encoding and media type.
    /// </summary>
    public static HttpRequestBuilder WithContent(
        this HttpRequestBuilder builder,
        string content,
        Encoding encoding,
        string mediaType)
    {
        builder.Content = new StringContent(content, encoding, mediaType);
        return builder;
    }

    /// <summary>
    /// Sets the request content using a <see cref="StringContent"/> and applies the specified <see cref="MediaTypeHeaderValue"/>.
    /// </summary>
    public static HttpRequestBuilder WithContent(
        this HttpRequestBuilder builder,
        string content,
        MediaTypeHeaderValue mediaTypeHeaderValue)
    {
        var sc = new StringContent(content);
        sc.Headers.ContentType = mediaTypeHeaderValue;
        builder.Content = sc;
        return builder;
    }

    /// <summary>
    /// Sets the request content using a <see cref="StringContent"/> with the specified encoding and applies the given <see cref="MediaTypeHeaderValue"/>.
    /// </summary>
    public static HttpRequestBuilder WithContent(
        this HttpRequestBuilder builder,
        string content,
        Encoding encoding,
        MediaTypeHeaderValue mediaTypeHeaderValue)
    {
        var sc = new StringContent(content, encoding);
        sc.Headers.ContentType = mediaTypeHeaderValue;
        builder.Content = sc;
        return builder;
    }

    #endregion

    #region XML String Content

    /// <summary>
    /// Sets the request content to the provided XML string using UTF-8 encoding and the default XML media type.
    /// </summary>
    public static HttpRequestBuilder WithXmlContent(
        this HttpRequestBuilder builder,
        string xml)
    {
        builder.Content = new StringContent(xml, Encoding.UTF8, FluentXmlSerializer.DefaultContentType);
        return builder;
    }

    /// <summary>
    /// Sets the request content to the provided XML string using the specified encoding and the default XML media type.
    /// </summary>
    public static HttpRequestBuilder WithXmlContent(
        this HttpRequestBuilder builder,
        string xml,
        Encoding encoding)
    {
        builder.Content = new StringContent(xml, encoding, FluentXmlSerializer.DefaultContentType);
        return builder;
    }

    /// <summary>
    /// Sets the request content to the provided XML string using UTF-8 encoding and the specified media type.
    /// </summary>
    public static HttpRequestBuilder WithXmlContent(
        this HttpRequestBuilder builder,
        string xml,
        string contentType)
    {
        builder.Content = new StringContent(xml, Encoding.UTF8, contentType);
        return builder;
    }

    /// <summary>
    /// Sets the request content to the provided XML string using UTF-8 encoding and applies the specified <see cref="MediaTypeHeaderValue"/>.
    /// </summary>
    public static HttpRequestBuilder WithXmlContent(
        this HttpRequestBuilder builder,
        string xml,
        MediaTypeHeaderValue contentTypeHeaderValue)
    {
        var sc = new StringContent(xml, Encoding.UTF8);
        sc.Headers.ContentType = contentTypeHeaderValue;
        builder.Content = sc;
        return builder;
    }

    /// <summary>
    /// Sets the request content to the provided XML string using the specified encoding and media type string.
    /// </summary>
    public static HttpRequestBuilder WithXmlContent(
        this HttpRequestBuilder builder,
        string xml,
        Encoding encoding,
        string contentType)
    {
        builder.Content = new StringContent(xml, encoding, contentType);
        return builder;
    }

    /// <summary>
    /// Sets the request content to the provided XML string using the specified encoding and applies the given <see cref="MediaTypeHeaderValue"/>.
    /// </summary>
    public static HttpRequestBuilder WithXmlContent(
        this HttpRequestBuilder builder,
        string xml,
        Encoding encoding,
        MediaTypeHeaderValue contentTypeHeaderValue)
    {
        var sc = new StringContent(xml, encoding);
        sc.Headers.ContentType = contentTypeHeaderValue;
        builder.Content = sc;
        return builder;
    }

    #endregion

    #region JSON String Content

    /// <summary>
    /// Sets the request content to the provided JSON string using UTF-8 encoding and the default JSON media type.
    /// </summary>
    public static HttpRequestBuilder WithJsonContent(
        this HttpRequestBuilder builder,
        string json)
    {
        builder.Content = new StringContent(json, Encoding.UTF8, FluentJsonSerializer.DefaultContentType);
        return builder;
    }

    /// <summary>
    /// Sets the request content to the provided JSON string using the specified encoding and the default JSON media type.
    /// </summary>
    public static HttpRequestBuilder WithJsonContent(
        this HttpRequestBuilder builder,
        string json,
        Encoding encoding)
    {
        builder.Content = new StringContent(json, encoding, FluentJsonSerializer.DefaultContentType);
        return builder;
    }

    /// <summary>
    /// Sets the request content to the provided JSON string using UTF-8 encoding and the specified media type.
    /// </summary>
    public static HttpRequestBuilder WithJsonContent(
        this HttpRequestBuilder builder,
        string json,
        string contentType)
    {
        builder.Content = new StringContent(json, Encoding.UTF8, contentType);
        return builder;
    }

    /// <summary>
    /// Sets the request content to the provided JSON string using the specified encoding and media type.
    /// </summary>
    public static HttpRequestBuilder WithJsonContent(
        this HttpRequestBuilder builder,
        string json,
        Encoding encoding,
        string contentType)
    {
        builder.Content = new StringContent(json, encoding, contentType);
        return builder;
    }

    /// <summary>
    /// Sets the request content to the provided JSON string using UTF-8 encoding and applies the specified content type header value.
    /// </summary>
    public static HttpRequestBuilder WithJsonContent(
        this HttpRequestBuilder builder,
        string json,
        MediaTypeHeaderValue contentTypeHeaderValue)
    {
        var sc = new StringContent(json, Encoding.UTF8);
        sc.Headers.ContentType = contentTypeHeaderValue;
        builder.Content = sc;
        return builder;
    }

    /// <summary>
    /// Sets the request content to the provided JSON string using the specified encoding and applies the given content type header value.
    /// </summary>
    public static HttpRequestBuilder WithJsonContent(
        this HttpRequestBuilder builder,
        string json,
        Encoding encoding,
        MediaTypeHeaderValue contentTypeHeaderValue)
    {
        var sc = new StringContent(json, encoding);
        sc.Headers.ContentType = contentTypeHeaderValue;
        builder.Content = sc;
        return builder;
    }

    #endregion
}
