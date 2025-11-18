using System.Text.Json;

#if NET6_0_OR_GREATER
using System.Text.Json.Nodes;
#endif

namespace FluentHttpClient;

/// <summary>
/// Fluent extension methods for deserializing JSON from an <see cref="HttpRequestMessage"/> instance.
/// </summary>
public static class FluentJsonDeserialization
{
    private static readonly JsonDocumentOptions _jsonDocumentOptions = new();
#if NET6_0_OR_GREATER
    private static readonly JsonNodeOptions _jsonNodeOptions = default;
#endif

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

    /// <summary>
    /// Reads the JSON content of the response and parses it into a <see cref="JsonDocument"/>.
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    public static Task<JsonDocument?> ReadJsonDocumentAsync(this HttpResponseMessage response)
    {
        return response.ReadJsonDocumentAsync(_jsonDocumentOptions, CancellationToken.None);
    }

    /// <summary>
    /// Reads the JSON content of the response and parses it into a <see cref="JsonDocument"/>,
    /// using the specified <see cref="JsonDocumentOptions"/>.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="documentOptions"></param>
    /// <returns></returns>
    public static Task<JsonDocument?> ReadJsonDocumentAsync(
        this HttpResponseMessage response,
        JsonDocumentOptions documentOptions)
    {
        return response.ReadJsonDocumentAsync(documentOptions, CancellationToken.None);
    }

    /// <summary>
    /// Reads the JSON content of the response and parses it into a <see cref="JsonDocument"/>,
    /// observing the provided cancellation token.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<JsonDocument?> ReadJsonDocumentAsync(
        this HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        return response.ReadJsonDocumentAsync(_jsonDocumentOptions, cancellationToken);
    }

    /// <summary>
    /// Reads the JSON content of the response and parses it into a <see cref="JsonDocument"/>,
    /// using the specified <see cref="JsonDocumentOptions"/> and cancellation token.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="documentOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<JsonDocument?> ReadJsonDocumentAsync(
        this HttpResponseMessage response,
        JsonDocumentOptions documentOptions,
        CancellationToken cancellationToken)
    {
        if (response.Content is null)
        {
            return null;
        }

#if NET5_0_OR_GREATER
        var stream = await response.Content
            .ReadAsStreamAsync(cancellationToken)
            .ConfigureAwait(false);
#else
        var stream = await response.Content
            .ReadAsStreamAsync()
            .ConfigureAwait(false);
#endif

        return JsonDocument.Parse(stream, documentOptions);
    }

    /// <summary>
    /// Awaits the HTTP response task, then reads the JSON content and parses it into a <see cref="JsonDocument"/>.
    /// </summary>
    /// <param name="responseTask"></param>
    /// <returns></returns>
    public static Task<JsonDocument?> ReadJsonDocumentAsync(this Task<HttpResponseMessage> responseTask)
    {
        return responseTask.ReadJsonDocumentAsync(_jsonDocumentOptions, CancellationToken.None);
    }

    /// <summary>
    /// Awaits the HTTP response task, then reads the JSON content and parses it into a <see cref="JsonDocument"/>,
    /// using the specified <see cref="JsonDocumentOptions"/>.
    /// </summary>
    /// <param name="responseTask"></param>
    /// <param name="documentOptions"></param>
    /// <returns></returns>
    public static Task<JsonDocument?> ReadJsonDocumentAsync(
        this Task<HttpResponseMessage> responseTask,
        JsonDocumentOptions documentOptions)
    {
        return responseTask.ReadJsonDocumentAsync(documentOptions, CancellationToken.None);
    }

    /// <summary>
    /// Awaits the HTTP response task, then reads the JSON content and parses it into a <see cref="JsonDocument"/>,
    /// observing the provided cancellation token.
    /// </summary>
    /// <param name="responseTask"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<JsonDocument?> ReadJsonDocumentAsync(
        this Task<HttpResponseMessage> responseTask,
        CancellationToken cancellationToken)
    {
        return responseTask.ReadJsonDocumentAsync(_jsonDocumentOptions, cancellationToken);
    }

    /// <summary>
    /// Awaits the HTTP response task, then reads the JSON content and parses it into a <see cref="JsonDocument"/>,
    /// using the specified <see cref="JsonDocumentOptions"/> and cancellation token.
    /// </summary>
    /// <param name="responseTask"></param>
    /// <param name="documentOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<JsonDocument?> ReadJsonDocumentAsync(
        this Task<HttpResponseMessage> responseTask,
        JsonDocumentOptions documentOptions,
        CancellationToken cancellationToken)
    {
        var response = await responseTask.ConfigureAwait(false);

        return await response
            .ReadJsonDocumentAsync(documentOptions, cancellationToken)
            .ConfigureAwait(false);
    }

#if NET6_0_OR_GREATER
    /// <summary>
    /// Reads the JSON content of the response and parses it into a <see cref="JsonObject"/>.
    /// </summary>
    /// <param name="response"></param>
    public static Task<JsonObject?> ReadJsonObjectAsync(this HttpResponseMessage response)
    {
        return response.ReadJsonObjectAsync(
            _jsonNodeOptions,
            _jsonDocumentOptions,
            CancellationToken.None);
    }

    /// <summary>
    /// Reads the JSON content of the response and parses it into a <see cref="JsonObject"/>,
    /// observing the provided cancellation token.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="cancellationToken"></param>
    public static Task<JsonObject?> ReadJsonObjectAsync(
        this HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        return response.ReadJsonObjectAsync(
            _jsonNodeOptions,
            _jsonDocumentOptions,
            cancellationToken);
    }

    /// <summary>
    /// Reads the JSON content of the response and parses it into a <see cref="JsonObject"/>,
    /// using the specified <see cref="JsonNodeOptions"/>.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="nodeOptions"></param>
    public static Task<JsonObject?> ReadJsonObjectAsync(
        this HttpResponseMessage response,
        JsonNodeOptions nodeOptions)
    {
        return response.ReadJsonObjectAsync(
            nodeOptions,
            _jsonDocumentOptions,
            CancellationToken.None);
    }

    /// <summary>
    /// Reads the JSON content of the response and parses it into a <see cref="JsonObject"/>,
    /// using the specified <see cref="JsonNodeOptions"/> and observing the provided cancellation token.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="nodeOptions"></param>
    /// <param name="cancellationToken"></param>
    public static Task<JsonObject?> ReadJsonObjectAsync(
        this HttpResponseMessage response,
        JsonNodeOptions nodeOptions,
        CancellationToken cancellationToken)
    {
        return response.ReadJsonObjectAsync(
            nodeOptions,
            _jsonDocumentOptions,
            cancellationToken);
    }

    /// <summary>
    /// Reads the JSON content of the response and parses it into a <see cref="JsonObject"/>,
    /// using the specified <see cref="JsonDocumentOptions"/>.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="documentOptions"></param>
    public static Task<JsonObject?> ReadJsonObjectAsync(
        this HttpResponseMessage response,
        JsonDocumentOptions documentOptions)
    {
        return response.ReadJsonObjectAsync(
            _jsonNodeOptions,
            documentOptions,
            CancellationToken.None);
    }

    /// <summary>
    /// Reads the JSON content of the response and parses it into a <see cref="JsonObject"/>,
    /// using the specified <see cref="JsonDocumentOptions"/> and observing the provided cancellation token.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="documentOptions"></param>
    /// <param name="cancellationToken"></param>
    public static Task<JsonObject?> ReadJsonObjectAsync(
        this HttpResponseMessage response,
        JsonDocumentOptions documentOptions,
        CancellationToken cancellationToken)
    {
        return response.ReadJsonObjectAsync(
            _jsonNodeOptions,
            documentOptions,
            cancellationToken);
    }

    /// <summary>
    /// Reads the JSON content of the response and parses it into a <see cref="JsonObject"/>,
    /// using the specified <see cref="JsonNodeOptions"/> and <see cref="JsonDocumentOptions"/>.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="nodeOptions"></param>
    /// <param name="documentOptions"></param>
    public static Task<JsonObject?> ReadJsonObjectAsync(
        this HttpResponseMessage response,
        JsonNodeOptions nodeOptions,
        JsonDocumentOptions documentOptions)
    {
        return response.ReadJsonObjectAsync(
            nodeOptions,
            documentOptions,
            CancellationToken.None);
    }

    /// <summary>
    /// Reads the JSON content of the response and parses it into a <see cref="JsonObject"/>,
    /// using the specified <see cref="JsonNodeOptions"/>, <see cref="JsonDocumentOptions"/>,
    /// and observing the provided cancellation token.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="nodeOptions"></param>
    /// <param name="documentOptions"></param>
    /// <param name="cancellationToken"></param>
    public static async Task<JsonObject?> ReadJsonObjectAsync(
        this HttpResponseMessage response,
        JsonNodeOptions nodeOptions,
        JsonDocumentOptions documentOptions,
        CancellationToken cancellationToken)
    {
        if (response.Content is null)
        {
            return null;
        }

        var stream = await response.Content
            .ReadAsStreamAsync(cancellationToken)
            .ConfigureAwait(false);

#if NET8_0_OR_GREATER
        var node = await JsonNode
            .ParseAsync(stream, nodeOptions, documentOptions, cancellationToken)
            .ConfigureAwait(false);
#else
        var node = JsonNode.Parse(stream, nodeOptions, documentOptions);
#endif
        return node as JsonObject;
    }

    /// <summary>
    /// Awaits the HTTP response task, then reads the JSON content and parses it into a <see cref="JsonObject"/>.
    /// </summary>
    /// <param name="responseTask"></param>
    public static Task<JsonObject?> ReadJsonObjectAsync(this Task<HttpResponseMessage> responseTask)
    {
        return responseTask.ReadJsonObjectAsync(
            _jsonNodeOptions,
            _jsonDocumentOptions,
            CancellationToken.None);
    }

    /// <summary>
    /// Awaits the HTTP response task, then reads the JSON content and parses it into a <see cref="JsonObject"/>,
    /// observing the provided cancellation token.
    /// </summary>
    /// <param name="responseTask"></param>
    /// <param name="cancellationToken"></param>
    public static Task<JsonObject?> ReadJsonObjectAsync(
        this Task<HttpResponseMessage> responseTask,
        CancellationToken cancellationToken)
    {
        return responseTask.ReadJsonObjectAsync(
            _jsonNodeOptions,
            _jsonDocumentOptions,
            cancellationToken);
    }

    /// <summary>
    /// Awaits the HTTP response task, then reads the JSON content and parses it into a <see cref="JsonObject"/>,
    /// using the specified <see cref="JsonNodeOptions"/>.
    /// </summary>
    /// <param name="responseTask"></param>
    /// <param name="nodeOptions"></param>
    public static Task<JsonObject?> ReadJsonObjectAsync(
        this Task<HttpResponseMessage> responseTask,
        JsonNodeOptions nodeOptions)
    {
        return responseTask.ReadJsonObjectAsync(
            nodeOptions,
            _jsonDocumentOptions,
            CancellationToken.None);
    }

    /// <summary>
    /// Awaits the HTTP response task, then reads the JSON content and parses it into a <see cref="JsonObject"/>,
    /// using the specified <see cref="JsonNodeOptions"/> and observing the provided cancellation token.
    /// </summary>
    /// <param name="responseTask"></param>
    /// <param name="nodeOptions"></param>
    /// <param name="cancellationToken"></param>
    public static Task<JsonObject?> ReadJsonObjectAsync(
        this Task<HttpResponseMessage> responseTask,
        JsonNodeOptions nodeOptions,
        CancellationToken cancellationToken)
    {
        return responseTask.ReadJsonObjectAsync(
            nodeOptions,
            _jsonDocumentOptions,
            cancellationToken);
    }

    /// <summary>
    /// Awaits the HTTP response task, then reads the JSON content and parses it into a <see cref="JsonObject"/>,
    /// using the specified <see cref="JsonDocumentOptions"/>.
    /// </summary>
    /// <param name="responseTask"></param>
    /// <param name="documentOptions"></param>
    public static Task<JsonObject?> ReadJsonObjectAsync(
        this Task<HttpResponseMessage> responseTask,
        JsonDocumentOptions documentOptions)
    {
        return responseTask.ReadJsonObjectAsync(
            _jsonNodeOptions,
            documentOptions,
            CancellationToken.None);
    }

    /// <summary>
    /// Awaits the HTTP response task, then reads the JSON content and parses it into a <see cref="JsonObject"/>,
    /// using the specified <see cref="JsonDocumentOptions"/> and observing the provided cancellation token.
    /// </summary>
    /// <param name="responseTask"></param>
    /// <param name="documentOptions"></param>
    /// <param name="cancellationToken"></param>
    public static Task<JsonObject?> ReadJsonObjectAsync(
        this Task<HttpResponseMessage> responseTask,
        JsonDocumentOptions documentOptions,
        CancellationToken cancellationToken)
    {
        return responseTask.ReadJsonObjectAsync(
            _jsonNodeOptions,
            documentOptions,
            cancellationToken);
    }

    /// <summary>
    /// Awaits the HTTP response task, then reads the JSON content and parses it into a <see cref="JsonObject"/>,
    /// using the specified <see cref="JsonNodeOptions"/> and <see cref="JsonDocumentOptions"/>.
    /// </summary>
    /// <param name="responseTask"></param>
    /// <param name="nodeOptions"></param>
    /// <param name="documentOptions"></param>
    public static Task<JsonObject?> ReadJsonObjectAsync(
        this Task<HttpResponseMessage> responseTask,
        JsonNodeOptions nodeOptions,
        JsonDocumentOptions documentOptions)
    {
        return responseTask.ReadJsonObjectAsync(
            nodeOptions,
            documentOptions,
            CancellationToken.None);
    }

    /// <summary>
    /// Awaits the HTTP response task, then reads the JSON content and parses it into a <see cref="JsonObject"/>,
    /// using the specified <see cref="JsonNodeOptions"/>, <see cref="JsonDocumentOptions"/>,
    /// and observing the provided cancellation token.
    /// </summary>
    /// <param name="responseTask"></param>
    /// <param name="nodeOptions"></param>
    /// <param name="documentOptions"></param>
    /// <param name="cancellationToken"></param>
    public static async Task<JsonObject?> ReadJsonObjectAsync(
        this Task<HttpResponseMessage> responseTask,
        JsonNodeOptions nodeOptions,
        JsonDocumentOptions documentOptions,
        CancellationToken cancellationToken)
    {
        var response = await responseTask.ConfigureAwait(false);

        return await response
            .ReadJsonObjectAsync(nodeOptions, documentOptions, cancellationToken)
            .ConfigureAwait(false);
    }
#endif
}

