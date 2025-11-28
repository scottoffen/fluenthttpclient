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
    /// <para>Buffering can have a significant memory impact for large payloads.</para>
    /// </remarks>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
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
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="content">The HTTP content to send with the request.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
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
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="data">The dictionary containing form data as key-value pairs.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
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
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="data">The sequence of key-value pairs containing form data.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
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
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="content">The string content to send with the request.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithContent(
        this HttpRequestBuilder builder,
        string content)
    {
        Guard.AgainstNull(content, nameof(content));
        return builder.WithContent(content, null, null, null);
    }

    /// <summary>
    /// Sets the request content using a <see cref="StringContent"/> created with the specified encoding.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="content">The string content to send with the request.</param>
    /// <param name="encoding">The encoding to use for the content.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithContent(
        this HttpRequestBuilder builder,
        string content,
        Encoding encoding)
    {
        Guard.AgainstNull(content, nameof(content));
        Guard.AgainstNull(encoding, nameof(encoding));
        return builder.WithContent(content, encoding, null, null);
    }

    /// <summary>
    /// Sets the request content using a <see cref="StringContent"/> with UTF-8 encoding and the specified media type.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="content">The string content to send with the request.</param>
    /// <param name="mediaType">The media type string (e.g., "application/json").</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithContent(
        this HttpRequestBuilder builder,
        string content,
        string mediaType)
    {
        Guard.AgainstNull(content, nameof(content));
        Guard.AgainstNull(mediaType, nameof(mediaType));
        return builder.WithContent(content, null, mediaType, null);
    }

    /// <summary>
    /// Sets the request content using a <see cref="StringContent"/> with the specified encoding and media type.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="content">The string content to send with the request.</param>
    /// <param name="encoding">The encoding to use for the content.</param>
    /// <param name="mediaType">The media type string (e.g., "application/json").</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithContent(
        this HttpRequestBuilder builder,
        string content,
        Encoding encoding,
        string mediaType)
    {
        Guard.AgainstNull(content, nameof(content));
        Guard.AgainstNull(encoding, nameof(encoding));
        Guard.AgainstNull(mediaType, nameof(mediaType));
        return builder.WithContent(content, encoding, mediaType, null);
    }

    /// <summary>
    /// Sets the request content using a <see cref="StringContent"/> and applies the specified <see cref="MediaTypeHeaderValue"/>.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="content">The string content to send with the request.</param>
    /// <param name="mediaTypeHeaderValue">The media type header value to apply to the content.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithContent(
        this HttpRequestBuilder builder,
        string content,
        MediaTypeHeaderValue mediaTypeHeaderValue)
    {
        Guard.AgainstNull(content, nameof(content));
        Guard.AgainstNull(mediaTypeHeaderValue, nameof(mediaTypeHeaderValue));
        return builder.WithContent(content, null, null, mediaTypeHeaderValue);
    }

    /// <summary>
    /// Sets the request content using a <see cref="StringContent"/> with the specified encoding and applies the given <see cref="MediaTypeHeaderValue"/>.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="content">The string content to send with the request.</param>
    /// <param name="encoding">The encoding to use for the content.</param>
    /// <param name="mediaTypeHeaderValue">The media type header value to apply to the content.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithContent(
        this HttpRequestBuilder builder,
        string content,
        Encoding encoding,
        MediaTypeHeaderValue mediaTypeHeaderValue)
    {
        Guard.AgainstNull(content, nameof(content));
        Guard.AgainstNull(encoding, nameof(encoding));
        Guard.AgainstNull(mediaTypeHeaderValue, nameof(mediaTypeHeaderValue));
        return builder.WithContent(content, encoding, null, mediaTypeHeaderValue);
    }

    private static HttpRequestBuilder WithContent(
        this HttpRequestBuilder builder,
        string content,
        Encoding? encoding,
        string? mediaType,
        MediaTypeHeaderValue? mediaTypeHeaderValue
    )
    {
        if (mediaType is not null)
        {
            builder.Content = new StringContent(content, encoding, mediaType);
        }
        else if (mediaTypeHeaderValue is not null)
        {
            var sc = new StringContent(content, encoding);
            sc.Headers.ContentType = mediaTypeHeaderValue;
            builder.Content = sc;
        }
        else
        {
            builder.Content = new StringContent(content, encoding);
        }

        return builder;
    }

    #endregion

    #region XML String Content

    /// <summary>
    /// Sets the request content to the provided XML string using UTF-8 encoding and the default XML media type.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="xml">The XML string content to send with the request.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithXmlContent(
        this HttpRequestBuilder builder,
        string xml)
    {
        Guard.AgainstNull(xml, nameof(xml));
        return builder.WithContent(xml, Encoding.UTF8, FluentXmlSerializer.DefaultContentType);
    }

    /// <summary>
    /// Sets the request content to the provided XML string using the specified encoding and the default XML media type.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="xml">The XML string content to send with the request.</param>
    /// <param name="encoding">The encoding to use for the content.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithXmlContent(
        this HttpRequestBuilder builder,
        string xml,
        Encoding encoding)
    {
        Guard.AgainstNull(xml, nameof(xml));
        return builder.WithContent(xml, encoding, FluentXmlSerializer.DefaultContentType);
    }

    /// <summary>
    /// Sets the request content to the provided XML string using UTF-8 encoding and the specified media type.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="xml">The XML string content to send with the request.</param>
    /// <param name="contentType">The media type string for the content.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithXmlContent(
        this HttpRequestBuilder builder,
        string xml,
        string contentType)
    {
        Guard.AgainstNull(xml, nameof(xml));
        return builder.WithContent(xml, Encoding.UTF8, contentType);
    }

    /// <summary>
    /// Sets the request content to the provided XML string using UTF-8 encoding and applies the specified <see cref="MediaTypeHeaderValue"/>.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="xml">The XML string content to send with the request.</param>
    /// <param name="contentTypeHeaderValue">The media type header value to apply to the content.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithXmlContent(
        this HttpRequestBuilder builder,
        string xml,
        MediaTypeHeaderValue contentTypeHeaderValue)
    {
        Guard.AgainstNull(xml, nameof(xml));
        return builder.WithContent(xml, Encoding.UTF8, contentTypeHeaderValue);
    }

    /// <summary>
    /// Sets the request content to the provided XML string using the specified encoding and media type string.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="xml">The XML string content to send with the request.</param>
    /// <param name="encoding">The encoding to use for the content.</param>
    /// <param name="contentType">The media type string for the content.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithXmlContent(
        this HttpRequestBuilder builder,
        string xml,
        Encoding encoding,
        string contentType)
    {
        Guard.AgainstNull(xml, nameof(xml));
        return builder.WithContent(xml, encoding, contentType);
    }

    /// <summary>
    /// Sets the request content to the provided XML string using the specified encoding and applies the given <see cref="MediaTypeHeaderValue"/>.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="xml">The XML string content to send with the request.</param>
    /// <param name="encoding">The encoding to use for the content.</param>
    /// <param name="contentTypeHeaderValue">The media type header value to apply to the content.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithXmlContent(
        this HttpRequestBuilder builder,
        string xml,
        Encoding encoding,
        MediaTypeHeaderValue contentTypeHeaderValue)
    {
        Guard.AgainstNull(xml, nameof(xml));
        return builder.WithContent(xml, encoding, contentTypeHeaderValue);
    }

    #endregion

    #region JSON String Content

    /// <summary>
    /// Sets the request content to the provided JSON string using UTF-8 encoding and the default JSON media type.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="json">The JSON string content to send with the request.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithJsonContent(
        this HttpRequestBuilder builder,
        string json)
    {
        Guard.AgainstNull(json, nameof(json));
        return builder.WithContent(json, Encoding.UTF8, FluentJsonSerializer.DefaultContentType);
    }

    /// <summary>
    /// Sets the request content to the provided JSON string using the specified encoding and the default JSON media type.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="json">The JSON string content to send with the request.</param>
    /// <param name="encoding">The encoding to use for the content.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithJsonContent(
        this HttpRequestBuilder builder,
        string json,
        Encoding encoding)
    {
        Guard.AgainstNull(json, nameof(json));
        return builder.WithContent(json, encoding, FluentJsonSerializer.DefaultContentType);
    }

    /// <summary>
    /// Sets the request content to the provided JSON string using UTF-8 encoding and the specified media type.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="json">The JSON string content to send with the request.</param>
    /// <param name="contentType">The media type string for the content.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithJsonContent(
        this HttpRequestBuilder builder,
        string json,
        string contentType)
    {
        Guard.AgainstNull(json, nameof(json));
        return builder.WithContent(json, Encoding.UTF8, contentType);
    }

    /// <summary>
    /// Sets the request content to the provided JSON string using the specified encoding and media type.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="json">The JSON string content to send with the request.</param>
    /// <param name="encoding">The encoding to use for the content.</param>
    /// <param name="contentType">The media type string for the content.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithJsonContent(
        this HttpRequestBuilder builder,
        string json,
        Encoding encoding,
        string contentType)
    {
        Guard.AgainstNull(json, nameof(json));
        return builder.WithContent(json, encoding, contentType);
    }

    /// <summary>
    /// Sets the request content to the provided JSON string using UTF-8 encoding and applies the specified content type header value.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="json">The JSON string content to send with the request.</param>
    /// <param name="contentTypeHeaderValue">The media type header value to apply to the content.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithJsonContent(
        this HttpRequestBuilder builder,
        string json,
        MediaTypeHeaderValue contentTypeHeaderValue)
    {
        Guard.AgainstNull(json, nameof(json));
        return builder.WithContent(json, Encoding.UTF8, contentTypeHeaderValue);
    }

    /// <summary>
    /// Sets the request content to the provided JSON string using the specified encoding and applies the given content type header value.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="json">The JSON string content to send with the request.</param>
    /// <param name="encoding">The encoding to use for the content.</param>
    /// <param name="contentTypeHeaderValue">The media type header value to apply to the content.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithJsonContent(
        this HttpRequestBuilder builder,
        string json,
        Encoding encoding,
        MediaTypeHeaderValue contentTypeHeaderValue)
    {
        Guard.AgainstNull(json, nameof(json));
        return builder.WithContent(json, encoding, contentTypeHeaderValue);
    }

    #endregion
}
