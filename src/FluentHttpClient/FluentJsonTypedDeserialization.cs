using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace FluentHttpClient;

/// <summary>
/// Fluent extension methods for deserializing JSON from an <see cref="HttpRequestMessage"/> instance.
/// </summary>
#if NET7_0_OR_GREATER
[RequiresDynamicCode("Uses reflection-based JSON deserialization. For Native AOT, use the overloads that accept JsonTypeInfo<T>.")]
#endif
#if NET6_0_OR_GREATER
[RequiresUnreferencedCode("Uses reflection-based JSON deserialization which is not trimming-safe. For trimmed/AOT apps, use the overloads that accept JsonTypeInfo<T>.")]
#endif
public static partial class FluentJsonTypedDeserialization
{
    /// <summary>
    /// Reads the JSON content of the response and deserializes it to <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="response"></param>
    /// <returns></returns>
    public static Task<T?> ReadJsonAsync<T>(this HttpResponseMessage response)
    {
        return response.ReadJsonAsync<T>(
            FluentJsonSerializer.DefaultJsonSerializerOptions,
            CancellationToken.None);
    }

    /// <summary>
    /// Reads the JSON content of the response and deserializes it to <typeparamref name="T"/>,
    /// using the specified serializer options.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="response"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static Task<T?> ReadJsonAsync<T>(
        this HttpResponseMessage response,
        JsonSerializerOptions? options)
    {
        return response.ReadJsonAsync<T>(options, CancellationToken.None);
    }

    /// <summary>
    /// Reads the JSON content of the response and deserializes it to <typeparamref name="T"/>,
    /// observing the provided cancellation token.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="response"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<T?> ReadJsonAsync<T>(
        this HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        return response.ReadJsonAsync<T>(
            FluentJsonSerializer.DefaultJsonSerializerOptions,
            cancellationToken);
    }

    /// <summary>
    /// Reads the JSON content of the response and deserializes it to <typeparamref name="T"/>,
    /// using the specified serializer options and cancellation token.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="response"></param>
    /// <param name="options"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<T?> ReadJsonAsync<T>(
        this HttpResponseMessage response,
        JsonSerializerOptions? options,
        CancellationToken cancellationToken)
    {
        // NOTE: HttpResponseMessage.Content is never null on modern TFMs, but can be on older platforms.
        // These checks exist for cross-target safety and are not hit in current test runs.
        if (response.Content is null)
        {
            return default;
        }

        options ??= FluentJsonSerializer.DefaultJsonSerializerOptions;

#if NET5_0_OR_GREATER
        var stream = await response.Content
            .ReadAsStreamAsync(cancellationToken)
            .ConfigureAwait(false);
#else
        var stream = await response.Content
            .ReadAsStreamAsync()
            .ConfigureAwait(false);
#endif

        return await JsonSerializer
            .DeserializeAsync<T>(stream, options, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Awaits the HTTP response task, then reads and deserializes the JSON content to <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseTask"></param>
    /// <returns></returns>
    public static Task<T?> ReadJsonAsync<T>(this Task<HttpResponseMessage> responseTask)
    {
        return responseTask.ReadJsonAsync<T>(
            FluentJsonSerializer.DefaultJsonSerializerOptions,
            CancellationToken.None);
    }

    /// <summary>
    /// Awaits the HTTP response task, then reads and deserializes the JSON content to <typeparamref name="T"/>,
    /// using the specified serializer options.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseTask"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static Task<T?> ReadJsonAsync<T>(
        this Task<HttpResponseMessage> responseTask,
        JsonSerializerOptions? options)
    {
        return responseTask.ReadJsonAsync<T>(options, CancellationToken.None);
    }

    /// <summary>
    /// Awaits the HTTP response task, then reads and deserializes the JSON content to <typeparamref name="T"/>,
    /// observing the provided cancellation token.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseTask"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<T?> ReadJsonAsync<T>(
        this Task<HttpResponseMessage> responseTask,
        CancellationToken cancellationToken)
    {
        return responseTask.ReadJsonAsync<T>(
            FluentJsonSerializer.DefaultJsonSerializerOptions,
            cancellationToken);
    }

    /// <summary>
    /// Awaits the HTTP response task, then reads and deserializes the JSON content to <typeparamref name="T"/>,
    /// using the specified serializer options and cancellation token.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseTask"></param>
    /// <param name="options"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<T?> ReadJsonAsync<T>(
        this Task<HttpResponseMessage> responseTask,
        JsonSerializerOptions? options,
        CancellationToken cancellationToken)
    {
        var response = await responseTask.ConfigureAwait(false);

        return await response
            .ReadJsonAsync<T>(options, cancellationToken)
            .ConfigureAwait(false);
    }
}
