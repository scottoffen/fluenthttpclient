using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;

namespace FluentHttpClient;

/// <summary>
/// Fluent extension methods for adding XML content to the TBuilder.
/// </summary>
#if NET7_0_OR_GREATER
[RequiresDynamicCode("XmlSerializer uses dynamic code generation which is not supported with Native AOT.")]
#endif
#if NET6_0_OR_GREATER
[RequiresUnreferencedCode("XML serialization using XmlSerializer may be incompatible with trimming. Ensure all required members are preserved or use these APIs only in non-trimmed scenarios.")]
#endif
public static class FluentXmlContentExtensions
{
    /// <summary>
    /// Serializes the specified value as XML using the default settings and sets it as the request content
    /// with UTF-8 encoding and the default XML media type.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder, which must inherit from <see cref="HttpRequestBuilder"/>.</typeparam>
    /// <typeparam name="T">The type of the value to serialize.</typeparam>
    /// <param name="builder">The TBuilder instance.</param>
    /// <param name="obj">The value to serialize as XML.</param>
    /// <returns>The TBuilder for method chaining.</returns>
    public static TBuilder WithXmlContent<TBuilder, T>(
        this TBuilder builder,
        T obj)
        where TBuilder : HttpRequestBuilder
        where T : class
    {
        var xml = FluentXmlSerializer.Serialize<T>(obj);
        builder.Content = new StringContent(xml, Encoding.UTF8, FluentXmlSerializer.DefaultContentType);
        return builder;
    }

    /// <summary>
    /// Serializes the specified value as XML using the provided settings and sets it as the request content
    /// with the encoding derived from the provided <see cref="XmlWriterSettings"/> and the default XML media type.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder, which must inherit from <see cref="HttpRequestBuilder"/>.</typeparam>
    /// <typeparam name="T">The type of the value to serialize.</typeparam>
    /// <param name="builder">The TBuilder instance.</param>
    /// <param name="obj">The value to serialize as XML.</param>
    /// <param name="settings">The XML writer settings to use during serialization.</param>
    /// <returns>The TBuilder for method chaining.</returns>
    public static TBuilder WithXmlContent<TBuilder, T>(
        this TBuilder builder,
        T obj,
        XmlWriterSettings settings)
        where TBuilder : HttpRequestBuilder
        where T : class
    {
        var xml = FluentXmlSerializer.Serialize<T>(obj, settings);
        var encoding = settings.Encoding ?? Encoding.UTF8;
        builder.Content = new StringContent(xml, encoding, FluentXmlSerializer.DefaultContentType);
        return builder;
    }

    /// <summary>
    /// Serializes the specified value as XML using the default settings and sets it as the request content
    /// with UTF-8 encoding and the specified media type.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder, which must inherit from <see cref="HttpRequestBuilder"/>.</typeparam>
    /// <typeparam name="T">The type of the value to serialize.</typeparam>
    /// <param name="builder">The TBuilder instance.</param>
    /// <param name="obj">The value to serialize as XML.</param>
    /// <param name="contentType">The media type string for the content.</param>
    /// <returns>The TBuilder for method chaining.</returns>
    public static TBuilder WithXmlContent<TBuilder, T>(
        this TBuilder builder,
        T obj,
        string contentType)
        where TBuilder : HttpRequestBuilder
        where T : class
    {
        var xml = FluentXmlSerializer.Serialize<T>(obj);
        builder.Content = new StringContent(xml, Encoding.UTF8, contentType);
        return builder;
    }

    /// <summary>
    /// Serializes the specified value as XML using the provided settings and sets it as the request content
    /// with the encoding derived from the provided <see cref="XmlWriterSettings"/> and the specified media type.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder, which must inherit from <see cref="HttpRequestBuilder"/>.</typeparam>
    /// <typeparam name="T">The type of the value to serialize.</typeparam>
    /// <param name="builder">The TBuilder instance.</param>
    /// <param name="obj">The value to serialize as XML.</param>
    /// <param name="settings">The XML writer settings to use during serialization.</param>
    /// <param name="contentType">The media type string for the content.</param>
    /// <returns>The TBuilder for method chaining.</returns>
    public static TBuilder WithXmlContent<TBuilder, T>(
        this TBuilder builder,
        T obj,
        XmlWriterSettings settings,
        string contentType)
        where TBuilder : HttpRequestBuilder
        where T : class
    {
        var xml = FluentXmlSerializer.Serialize<T>(obj, settings);
        var encoding = settings.Encoding ?? Encoding.UTF8;
        builder.Content = new StringContent(xml, encoding, contentType);
        return builder;
    }

    /// <summary>
    /// Serializes the specified value as XML using the default settings and sets it as the request content
    /// with UTF-8 encoding and applies the specified <see cref="MediaTypeHeaderValue"/>.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder, which must inherit from <see cref="HttpRequestBuilder"/>.</typeparam>
    /// <typeparam name="T">The type of the value to serialize.</typeparam>
    /// <param name="builder">The TBuilder instance.</param>
    /// <param name="obj">The value to serialize as XML.</param>
    /// <param name="contentTypeHeaderValue">The media type header value to apply to the content.</param>
    /// <returns>The TBuilder for method chaining.</returns>
    public static TBuilder WithXmlContent<TBuilder, T>(
        this TBuilder builder,
        T obj,
        MediaTypeHeaderValue contentTypeHeaderValue)
        where TBuilder : HttpRequestBuilder
        where T : class
    {
        var xml = FluentXmlSerializer.Serialize<T>(obj);
        var sc = new StringContent(xml, Encoding.UTF8);
        sc.Headers.ContentType = contentTypeHeaderValue;
        builder.Content = sc;
        return builder;
    }

    /// <summary>
    /// Serializes the specified value as XML using the provided settings and sets it as the request content
    /// with the encoding derived from the provided <see cref="XmlWriterSettings"/> and applies the given
    /// <see cref="MediaTypeHeaderValue"/>.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder, which must inherit from <see cref="HttpRequestBuilder"/>.</typeparam>
    /// <typeparam name="T">The type of the value to serialize.</typeparam>
    /// <param name="builder">The TBuilder instance.</param>
    /// <param name="obj">The value to serialize as XML.</param>
    /// <param name="settings">The XML writer settings to use during serialization.</param>
    /// <param name="contentTypeHeaderValue">The media type header value to apply to the content.</param>
    /// <returns>The TBuilder for method chaining.</returns>
    public static TBuilder WithXmlContent<TBuilder, T>(
        this TBuilder builder,
        T obj,
        XmlWriterSettings settings,
        MediaTypeHeaderValue contentTypeHeaderValue)
        where TBuilder : HttpRequestBuilder
        where T : class
    {
        var xml = FluentXmlSerializer.Serialize<T>(obj, settings);
        var encoding = settings.Encoding ?? Encoding.UTF8;
        var sc = new StringContent(xml, encoding);
        sc.Headers.ContentType = contentTypeHeaderValue;
        builder.Content = sc;
        return builder;
    }
}