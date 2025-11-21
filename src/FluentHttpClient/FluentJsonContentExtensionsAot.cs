#if NET7_0_OR_GREATER
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace FluentHttpClient;

public static partial class FluentJsonContentExtensions
{
    /// <summary>
    /// Serializes <paramref name="value"/> using the supplied <see cref="JsonTypeInfo{T}"/> and
    /// sets the JSON payload as the request content using UTF-8 and the default media type.
    /// </summary>
    public static HttpRequestBuilder WithJsonContent<T>(
        this HttpRequestBuilder builder,
        T value,
        JsonTypeInfo<T> jsonTypeInfo)
        where T : class
    {
        Guard.AgainstNull(builder, nameof(builder));
        Guard.AgainstNull(jsonTypeInfo, nameof(jsonTypeInfo));

        var json = JsonSerializer.Serialize(value, jsonTypeInfo);
        builder.Content = new StringContent(json, Encoding.UTF8, FluentJsonSerializer.DefaultContentType);
        return builder;
    }

    /// <summary>
    /// Serializes <paramref name="value"/> using metadata from the provided
    /// <see cref="JsonSerializerContext"/> and sets the JSON payload as the request content.
    /// </summary>
    public static HttpRequestBuilder WithJsonContent<T>(
        this HttpRequestBuilder builder,
        T value,
        JsonSerializerContext context)
        where T : class
    {
        Guard.AgainstNull(builder, nameof(builder));
        Guard.AgainstNull(context, nameof(context));

        var typeInfo = context.GetTypeInfo(typeof(T));
        if (typeInfo is not JsonTypeInfo<T> jsonTypeInfo)
        {
            throw new InvalidOperationException(
                $"The provided JsonSerializerContext does not contain metadata for type '{typeof(T)}'.");
        }

        return builder.WithJsonContent(value, jsonTypeInfo);
    }

    /// <summary>
    /// Serializes <paramref name="value"/> using the supplied <see cref="JsonTypeInfo{T}"/> and
    /// sets the JSON payload as the request content using UTF-8 and the specified media type.
    /// </summary>
    public static HttpRequestBuilder WithJsonContent<T>(
        this HttpRequestBuilder builder,
        T value,
        JsonTypeInfo<T> jsonTypeInfo,
        string contentType)
        where T : class
    {
        Guard.AgainstNull(builder, nameof(builder));
        Guard.AgainstNull(jsonTypeInfo, nameof(jsonTypeInfo));
        Guard.AgainstNull(contentType, nameof(contentType));

        var json = JsonSerializer.Serialize(value, jsonTypeInfo);
        builder.Content = new StringContent(json, Encoding.UTF8, contentType);
        return builder;
    }

    /// <summary>
    /// Serializes <paramref name="value"/> using metadata from the provided
    /// <see cref="JsonSerializerContext"/> and sets the JSON payload as the request content
    /// using UTF-8 and the specified media type.
    /// </summary>
    public static HttpRequestBuilder WithJsonContent<T>(
        this HttpRequestBuilder builder,
        T value,
        JsonSerializerContext context,
        string contentType)
        where T : class
    {
        Guard.AgainstNull(builder, nameof(builder));
        Guard.AgainstNull(context, nameof(context));
        Guard.AgainstNull(contentType, nameof(contentType));

        var typeInfo = context.GetTypeInfo(typeof(T));
        if (typeInfo is not JsonTypeInfo<T> jsonTypeInfo)
        {
            throw new InvalidOperationException(
                $"The provided JsonSerializerContext does not contain metadata for type '{typeof(T)}'.");
        }

        return builder.WithJsonContent(value, jsonTypeInfo, contentType);
    }

    /// <summary>
    /// Serializes <paramref name="value"/> using the supplied <see cref="JsonTypeInfo{T}"/> and
    /// sets the JSON payload as the request content using UTF-8 and the specified content type header.
    /// </summary>
    public static HttpRequestBuilder WithJsonContent<T>(
        this HttpRequestBuilder builder,
        T value,
        JsonTypeInfo<T> jsonTypeInfo,
        MediaTypeHeaderValue contentTypeHeaderValue)
        where T : class
    {
        Guard.AgainstNull(builder, nameof(builder));
        Guard.AgainstNull(jsonTypeInfo, nameof(jsonTypeInfo));
        Guard.AgainstNull(contentTypeHeaderValue, nameof(contentTypeHeaderValue));

        var json = JsonSerializer.Serialize(value, jsonTypeInfo);
        var sc = new StringContent(json, Encoding.UTF8);
        sc.Headers.ContentType = contentTypeHeaderValue;
        builder.Content = sc;

        return builder;
    }

    /// <summary>
    /// Serializes <paramref name="value"/> using metadata from the provided
    /// <see cref="JsonSerializerContext"/> and sets the JSON payload as the request content
    /// using UTF-8 and the specified content type header.
    /// </summary>
    public static HttpRequestBuilder WithJsonContent<T>(
        this HttpRequestBuilder builder,
        T value,
        JsonSerializerContext context,
        MediaTypeHeaderValue contentTypeHeaderValue)
        where T : class
    {
        Guard.AgainstNull(builder, nameof(builder));
        Guard.AgainstNull(context, nameof(context));
        Guard.AgainstNull(contentTypeHeaderValue, nameof(contentTypeHeaderValue));

        var typeInfo = context.GetTypeInfo(typeof(T));
        if (typeInfo is not JsonTypeInfo<T> jsonTypeInfo)
        {
            throw new InvalidOperationException(
                $"The provided JsonSerializerContext does not contain metadata for type '{typeof(T)}'.");
        }

        return builder.WithJsonContent(value, jsonTypeInfo, contentTypeHeaderValue);
    }
}
#endif
