#if NET7_0_OR_GREATER
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace FluentHttpClient;

public static partial class FluentJsonTypedDeserialization
{
    /// <summary>
    /// Reads the JSON content of the response and deserializes it to <typeparamref name="T"/>
    /// using the provided <see cref="JsonTypeInfo{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the JSON content into.</typeparam>
    /// <param name="response">The HTTP response whose content will be read.</param>
    /// <param name="jsonTypeInfo">The JSON type metadata for AOT-safe deserialization.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized object, or the default value if the content is empty.</returns>
    public static async Task<T?> ReadJsonAsync<T>(
        this HttpResponseMessage response,
        JsonTypeInfo<T> jsonTypeInfo,
        CancellationToken cancellationToken = default)
    {
        Guard.AgainstNull(response, nameof(response));
        Guard.AgainstNull(jsonTypeInfo, nameof(jsonTypeInfo));

        if (response.Content is null)
        {
            return default;
        }

        var json = await response.Content
            .ReadAsStringAsync(cancellationToken)
            .ConfigureAwait(false);

        if (string.IsNullOrEmpty(json)) return default;

        return JsonSerializer.Deserialize(json, jsonTypeInfo);
    }

    /// <summary>
    /// Reads the JSON content of the response and deserializes it to <typeparamref name="T"/>
    /// using the provided <see cref="JsonSerializerContext"/>.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the JSON content into.</typeparam>
    /// <param name="response">The HTTP response whose content will be read.</param>
    /// <param name="context">The JSON serializer context containing type metadata for AOT-safe deserialization.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized object, or the default value if the content is empty.</returns>
    public static Task<T?> ReadJsonAsync<T>(
        this HttpResponseMessage response,
        JsonSerializerContext context,
        CancellationToken cancellationToken = default)
    {
        Guard.AgainstNull(context, nameof(context));

        var typeInfo = context.GetTypeInfo(typeof(T));
        if (typeInfo is not JsonTypeInfo<T> jsonTypeInfo)
        {
            throw new InvalidOperationException(
                $"The provided JsonSerializerContext does not contain metadata for type '{typeof(T)}'.");
        }

        return response.ReadJsonAsync(jsonTypeInfo, cancellationToken);
    }

    /// <summary>
    /// Awaits the HTTP response task, then reads and deserializes the JSON content to <typeparamref name="T"/>
    /// using the provided <see cref="JsonTypeInfo{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the JSON content into.</typeparam>
    /// <param name="responseTask">A task that produces the HTTP response whose content will be read.</param>
    /// <param name="jsonTypeInfo">The JSON type metadata for AOT-safe deserialization.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized object, or the default value if the content is empty.</returns>
    public static async Task<T?> ReadJsonAsync<T>(
        this Task<HttpResponseMessage> responseTask,
        JsonTypeInfo<T> jsonTypeInfo,
        CancellationToken cancellationToken = default)
    {
        Guard.AgainstNull(responseTask, nameof(responseTask));
        Guard.AgainstNull(jsonTypeInfo, nameof(jsonTypeInfo));

        var response = await responseTask.ConfigureAwait(false);

        return await response
            .ReadJsonAsync(jsonTypeInfo, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Awaits the HTTP response task, then reads and deserializes the JSON content to <typeparamref name="T"/>
    /// using the provided <see cref="JsonSerializerContext"/>.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the JSON content into.</typeparam>
    /// <param name="responseTask">A task that produces the HTTP response whose content will be read.</param>
    /// <param name="context">The JSON serializer context containing type metadata for AOT-safe deserialization.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized object, or the default value if the content is empty.</returns>
    public static async Task<T?> ReadJsonAsync<T>(
        this Task<HttpResponseMessage> responseTask,
        JsonSerializerContext context,
        CancellationToken cancellationToken = default)
    {
        Guard.AgainstNull(responseTask, nameof(responseTask));
        Guard.AgainstNull(context, nameof(context));

        var response = await responseTask.ConfigureAwait(false);

        return await response
            .ReadJsonAsync<T>(context, cancellationToken)
            .ConfigureAwait(false);
    }
}
#endif
