using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FluentHttpClient;

/// <summary>
/// Fluent extension methods for adding JSON content to the <see cref="HttpRequestBuilder"/>.
/// </summary>
#if NET7_0_OR_GREATER
[RequiresDynamicCode("Uses reflection-based JSON deserialization. For Native AOT, use the overloads that accept JsonTypeInfo<T>.")]
#endif
#if NET6_0_OR_GREATER
[RequiresUnreferencedCode("Uses reflection-based JSON deserialization which is not trimming-safe. For trimmed/AOT apps, use the overloads that accept JsonTypeInfo<T>.")]
#endif
public static partial class FluentJsonContentExtensions
{
    /// <summary>
    /// Serializes the specified value as JSON using the default serializer options and sets it as the request content
    /// with UTF-8 encoding and the default JSON media type.
    /// </summary>
    public static HttpRequestBuilder WithJsonContent<T>(
        this HttpRequestBuilder builder,
        T value)
        where T : class
    {
        var json = JsonSerializer.Serialize(value, FluentJsonSerializer.DefaultJsonSerializerOptions);
        builder.Content = new StringContent(json, Encoding.UTF8, FluentJsonSerializer.DefaultContentType);
        return builder;
    }

    /// <summary>
    /// Serializes the specified value as JSON using the provided serializer options and sets it as the request content
    /// with UTF-8 encoding and the default JSON media type.
    /// </summary>
    public static HttpRequestBuilder WithJsonContent<T>(
        this HttpRequestBuilder builder,
        T value,
        JsonSerializerOptions options)
        where T : class
    {
        var json = JsonSerializer.Serialize(value, options);
        builder.Content = new StringContent(json, Encoding.UTF8, FluentJsonSerializer.DefaultContentType);
        return builder;
    }

    /// <summary>
    /// Serializes the specified value as JSON using the default serializer options and sets it as the request content
    /// with UTF-8 encoding and the specified media type.
    /// </summary>
    public static HttpRequestBuilder WithJsonContent<T>(
        this HttpRequestBuilder builder,
        T value,
        string contentType)
        where T : class
    {
        var json = JsonSerializer.Serialize(value, FluentJsonSerializer.DefaultJsonSerializerOptions);
        builder.Content = new StringContent(json, Encoding.UTF8, contentType);
        return builder;
    }

    /// <summary>
    /// Serializes the specified value as JSON using the provided serializer options and sets it as the request content
    /// with UTF-8 encoding and the specified media type.
    /// </summary>
    public static HttpRequestBuilder WithJsonContent<T>(
        this HttpRequestBuilder builder,
        T value,
        JsonSerializerOptions options,
        string contentType)
        where T : class
    {
        var json = JsonSerializer.Serialize(value, options);
        builder.Content = new StringContent(json, Encoding.UTF8, contentType);
        return builder;
    }

    /// <summary>
    /// Serializes the specified value as JSON using the default serializer options and sets it as the request content
    /// with UTF-8 encoding and applies the specified content type header value.
    /// </summary>
    public static HttpRequestBuilder WithJsonContent<T>(
        this HttpRequestBuilder builder,
        T value,
        MediaTypeHeaderValue contentTypeHeaderValue)
        where T : class
    {
        var json = JsonSerializer.Serialize(value, FluentJsonSerializer.DefaultJsonSerializerOptions);
        var sc = new StringContent(json, Encoding.UTF8);
        sc.Headers.ContentType = contentTypeHeaderValue;
        builder.Content = sc;
        return builder;
    }

    /// <summary>
    /// Serializes the specified value as JSON using the provided serializer options and sets it as the request content
    /// with UTF-8 encoding and applies the given content type header value.
    /// </summary>
    public static HttpRequestBuilder WithJsonContent<T>(
        this HttpRequestBuilder builder,
        T value,
        JsonSerializerOptions options,
        MediaTypeHeaderValue contentTypeHeaderValue)
        where T : class
    {
        var json = JsonSerializer.Serialize(value, options);
        var sc = new StringContent(json, Encoding.UTF8);
        sc.Headers.ContentType = contentTypeHeaderValue;
        builder.Content = sc;
        return builder;
    }
}
