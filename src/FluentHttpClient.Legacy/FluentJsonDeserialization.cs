using System.Text.Json;

namespace FluentHttpClient;

/// <summary>
/// Extension methods for JSON deserialization.
/// </summary>
public static class FluentJsonDeserialization
{
    #region StringDeserialization

    /// <summary>
    /// Parses the text representing a single JSON value into an instance of the type specified by a generic type parameter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns>A T representation of the JSON value.</returns>
    /// <exception cref="JsonException" />
    /// <exception cref="NotSupportedException" />
    /// <exception cref="ArgumentNullException" />
    public static T? DeserializeJson<T>(this string value)
    {
        return value.DeserializeJson<T>(FluentHttpClientOptions.DefaultJsonSerializerOptions);
    }

    /// <summary>
    /// Parses the text representing a single JSON value into an instance of the type specified by a generic type parameter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <returns>A T representation of the JSON value.</returns>
    /// <exception cref="JsonException" />
    /// <exception cref="NotSupportedException" />
    /// <exception cref="ArgumentNullException" />
    public static T? DeserializeJson<T>(this string value, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<T>(value, options);
    }

    /// <summary>
    /// Parses the text representing a single JSON value into an instance of the type specified by a generic type parameter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stringTask"></param>
    /// <returns>A T representation of the JSON value.</returns>
    /// <exception cref="JsonException" />
    /// <exception cref="NotSupportedException" />
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<string> stringTask)
    {
        return await stringTask
            .DeserializeJsonAsync<T>(FluentHttpClientOptions.DefaultJsonSerializerOptions)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Parses the text representing a single JSON value into an instance of the type specified by a generic type parameter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stringTask"></param>
    /// <param name="options"></param>
    /// <returns>A T representation of the JSON value.</returns>
    /// <exception cref="JsonException" />
    /// <exception cref="NotSupportedException" />
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<string> stringTask, JsonSerializerOptions options)
    {
        var value = await stringTask.ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(value, options);
    }

    #endregion StringDeserialization

    #region TaskHttpResponseMessageDeserialization

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text of <see cref="HttpResponseMessage.Content"/> representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessageTask"></param>
    /// <param name="defaultAction"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<HttpResponseMessage> responseMessageTask, Func<HttpResponseMessage, Exception, Task<T>> defaultAction)
    {
        return await responseMessageTask
            .DeserializeJsonAsync<T>(defaultAction, FluentHttpClientOptions.DefaultJsonSerializerOptions, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text of <see cref="HttpResponseMessage.Content"/> representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessageTask"></param>
    /// <param name="defaultAction"></param>
    /// <param name="options"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<HttpResponseMessage> responseMessageTask, Func<HttpResponseMessage, Exception, Task<T>> defaultAction, JsonSerializerOptions options)
    {
        return await responseMessageTask
            .DeserializeJsonAsync<T>(defaultAction, options, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text of <see cref="HttpResponseMessage.Content"/> representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessageTask"></param>
    /// <param name="defaultAction"></param>
    /// <param name="token"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<HttpResponseMessage> responseMessageTask, Func<HttpResponseMessage, Exception, Task<T>> defaultAction, CancellationToken token)
    {
        return await responseMessageTask
            .DeserializeJsonAsync<T>(defaultAction, FluentHttpClientOptions.DefaultJsonSerializerOptions, token)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text of <see cref="HttpResponseMessage.Content"/> representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessageTask"></param>
    /// <param name="defaultAction"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<HttpResponseMessage> responseMessageTask, Func<HttpResponseMessage, Exception, Task<T>> defaultAction, JsonSerializerOptions options, CancellationToken token)
    {
        if (defaultAction == null) throw new ArgumentNullException(nameof(defaultAction));

        try
        {
            return await responseMessageTask
                .DeserializeJsonAsync<T>(options, token)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            var responseMessage = await responseMessageTask.ConfigureAwait(false);
            return await defaultAction(responseMessage, e).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text of <see cref="HttpResponseMessage.Content"/> representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessageTask"></param>
    /// <param name="defaultAction"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<HttpResponseMessage> responseMessageTask, Func<HttpResponseMessage, Exception, T> defaultAction)
    {
        return await responseMessageTask
            .DeserializeJsonAsync<T>(defaultAction, FluentHttpClientOptions.DefaultJsonSerializerOptions, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text of <see cref="HttpResponseMessage.Content"/> representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessageTask"></param>
    /// <param name="defaultAction"></param>
    /// <param name="options"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<HttpResponseMessage> responseMessageTask, Func<HttpResponseMessage, Exception, T> defaultAction, JsonSerializerOptions options)
    {
        return await responseMessageTask
            .DeserializeJsonAsync<T>(defaultAction, options, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text of <see cref="HttpResponseMessage.Content"/> representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessageTask"></param>
    /// <param name="defaultAction"></param>
    /// <param name="token"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<HttpResponseMessage> responseMessageTask, Func<HttpResponseMessage, Exception, T> defaultAction, CancellationToken token)
    {
        return await responseMessageTask
            .DeserializeJsonAsync<T>(defaultAction, FluentHttpClientOptions.DefaultJsonSerializerOptions, token)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text of <see cref="HttpResponseMessage.Content"/> representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessageTask"></param>
    /// <param name="defaultAction"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<HttpResponseMessage> responseMessageTask, Func<HttpResponseMessage, Exception, T> defaultAction, JsonSerializerOptions options, CancellationToken token)
    {
        if (defaultAction == null) throw new ArgumentNullException(nameof(defaultAction));

        try
        {
            return await responseMessageTask
                .DeserializeJsonAsync<T>(options, token)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            var responseMessage = await responseMessageTask.ConfigureAwait(false);
            return defaultAction(responseMessage, e);
        }
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text of <see cref="HttpResponseMessage.Content"/> representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessageTask"></param>
    /// <returns>A T representation of the JSON value.</returns>
    /// <exception cref="JsonException" />
    /// <exception cref="NotSupportedException" />
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<HttpResponseMessage> responseMessageTask)
    {
        return await responseMessageTask
            .DeserializeJsonAsync<T>(FluentHttpClientOptions.DefaultJsonSerializerOptions, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text of <see cref="HttpResponseMessage.Content"/> representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessageTask"></param>
    /// <param name="options"></param>
    /// <returns>A T representation of the JSON value.</returns>
    /// <exception cref="JsonException" />
    /// <exception cref="NotSupportedException" />
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<HttpResponseMessage> responseMessageTask, JsonSerializerOptions options)
    {
        return await responseMessageTask
            .DeserializeJsonAsync<T>(options, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text of <see cref="HttpResponseMessage.Content"/> representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessageTask"></param>
    /// <param name="token"></param>
    /// <returns>A T representation of the JSON value.</returns>
    /// <exception cref="JsonException" />
    /// <exception cref="NotSupportedException" />
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<HttpResponseMessage> responseMessageTask, CancellationToken token)
    {
        return await responseMessageTask
            .DeserializeJsonAsync<T>(FluentHttpClientOptions.DefaultJsonSerializerOptions, token)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text of <see cref="HttpResponseMessage.Content"/> representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessageTask"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    /// <returns>A T representation of the JSON value.</returns>
    /// <exception cref="JsonException" />
    /// <exception cref="NotSupportedException" />
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<HttpResponseMessage> responseMessageTask, JsonSerializerOptions options, CancellationToken token)
    {
        return await responseMessageTask
            .GetResponseStreamAsync()
            .DeserializeJsonAsync<T>(options, token)
            .ConfigureAwait(false);
    }

    #endregion TaskHttpResponseMessageDeserialization

    #region HttpResponseMessageDeserialization

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text of <see cref="HttpResponseMessage.Content"/> representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessage"></param>
    /// <param name="defaultAction"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this HttpResponseMessage responseMessage, Func<HttpResponseMessage, Exception, Task<T>> defaultAction)
    {
        return await responseMessage
            .DeserializeJsonAsync<T>(defaultAction, FluentHttpClientOptions.DefaultJsonSerializerOptions, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text of <see cref="HttpResponseMessage.Content"/> representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessage"></param>
    /// <param name="defaultAction"></param>
    /// <param name="options"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this HttpResponseMessage responseMessage, Func<HttpResponseMessage, Exception, Task<T>> defaultAction, JsonSerializerOptions options)
    {
        return await responseMessage
            .DeserializeJsonAsync<T>(defaultAction, options, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text of <see cref="HttpResponseMessage.Content"/> representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessage"></param>
    /// <param name="defaultAction"></param>
    /// <param name="token"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this HttpResponseMessage responseMessage, Func<HttpResponseMessage, Exception, Task<T>> defaultAction, CancellationToken token)
    {
        return await responseMessage
            .DeserializeJsonAsync<T>(defaultAction, FluentHttpClientOptions.DefaultJsonSerializerOptions, token)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text of <see cref="HttpResponseMessage.Content"/> representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessage"></param>
    /// <param name="defaultAction"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this HttpResponseMessage responseMessage, Func<HttpResponseMessage, Exception, Task<T>> defaultAction, JsonSerializerOptions options, CancellationToken token)
    {
        if (defaultAction == null) throw new ArgumentNullException(nameof(defaultAction));

        try
        {
            return await responseMessage
                .DeserializeJsonAsync<T>(options, token)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            return await defaultAction
                .Invoke(responseMessage, e)
                .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text of <see cref="HttpResponseMessage.Content"/> representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessage"></param>
    /// <param name="defaultAction"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this HttpResponseMessage responseMessage, Func<HttpResponseMessage, Exception, T> defaultAction)
    {
        return await responseMessage
            .DeserializeJsonAsync<T>(defaultAction, FluentHttpClientOptions.DefaultJsonSerializerOptions, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text of <see cref="HttpResponseMessage.Content"/> representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessage"></param>
    /// <param name="defaultAction"></param>
    /// <param name="options"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this HttpResponseMessage responseMessage, Func<HttpResponseMessage, Exception, T> defaultAction, JsonSerializerOptions options)
    {
        return await responseMessage
            .DeserializeJsonAsync<T>(defaultAction, options, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text of <see cref="HttpResponseMessage.Content"/> representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessage"></param>
    /// <param name="defaultAction"></param>
    /// <param name="token"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this HttpResponseMessage responseMessage, Func<HttpResponseMessage, Exception, T> defaultAction, CancellationToken token)
    {
        return await responseMessage
            .DeserializeJsonAsync<T>(defaultAction, FluentHttpClientOptions.DefaultJsonSerializerOptions, token)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text of <see cref="HttpResponseMessage.Content"/> representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessage"></param>
    /// <param name="defaultAction"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this HttpResponseMessage responseMessage, Func<HttpResponseMessage, Exception, T> defaultAction, JsonSerializerOptions options, CancellationToken token)
    {
        if (defaultAction == null) throw new ArgumentNullException(nameof(defaultAction));

        try
        {
            return await responseMessage
                .DeserializeJsonAsync<T>(options, token)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            return defaultAction.Invoke(responseMessage, e);
        }
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text of <see cref="HttpResponseMessage.Content"/> representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessage"></param>
    /// <returns>A T representation of the JSON value.</returns>
    /// <exception cref="JsonException" />
    /// <exception cref="NotSupportedException" />
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this HttpResponseMessage responseMessage)
    {
        return await responseMessage
            .DeserializeJsonAsync<T>(FluentHttpClientOptions.DefaultJsonSerializerOptions, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text of <see cref="HttpResponseMessage.Content"/> representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessage"></param>
    /// <param name="options"></param>
    /// <returns>A T representation of the JSON value.</returns>
    /// <exception cref="JsonException" />
    /// <exception cref="NotSupportedException" />
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this HttpResponseMessage responseMessage, JsonSerializerOptions options)
    {
        return await responseMessage
            .DeserializeJsonAsync<T>(options, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text of <see cref="HttpResponseMessage.Content"/> representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessage"></param>
    /// <param name="token"></param>
    /// <returns>A T representation of the JSON value.</returns>
    /// <exception cref="JsonException" />
    /// <exception cref="NotSupportedException" />
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this HttpResponseMessage responseMessage, CancellationToken token)
    {
        return await responseMessage
            .DeserializeJsonAsync<T>(FluentHttpClientOptions.DefaultJsonSerializerOptions, token)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text of <see cref="HttpResponseMessage.Content"/> representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessage"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    /// <returns>A T representation of the JSON value.</returns>
    /// <exception cref="JsonException" />
    /// <exception cref="NotSupportedException" />
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this HttpResponseMessage responseMessage, JsonSerializerOptions options, CancellationToken token)
    {
        return await responseMessage
            .GetResponseStreamAsync()
            .DeserializeJsonAsync<T>(options, token)
            .ConfigureAwait(false);
    }

    #endregion HttpResponseMessageDeserialization

    #region TaskStreamDeserialization

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="streamTask"></param>
    /// <param name="defaultAction"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<Stream> streamTask, Func<Exception, Task<T>> defaultAction)
    {
        return await streamTask
            .DeserializeJsonAsync<T>(defaultAction, FluentHttpClientOptions.DefaultJsonSerializerOptions, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="streamTask"></param>
    /// <param name="defaultAction"></param>
    /// <param name="options"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<Stream> streamTask, Func<Exception, Task<T>> defaultAction, JsonSerializerOptions options)
    {
        return await streamTask
            .DeserializeJsonAsync<T>(defaultAction, options, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="streamTask"></param>
    /// <param name="defaultAction"></param>
    /// <param name="token"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<Stream> streamTask, Func<Exception, Task<T>> defaultAction, CancellationToken token)
    {
        return await streamTask
            .DeserializeJsonAsync<T>(defaultAction, FluentHttpClientOptions.DefaultJsonSerializerOptions, token)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="streamTask"></param>
    /// <param name="defaultAction"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<Stream> streamTask, Func<Exception, Task<T>> defaultAction, JsonSerializerOptions options, CancellationToken token)
    {
        if (defaultAction == null) throw new ArgumentNullException(nameof(defaultAction));

        try
        {
            return await streamTask
                .DeserializeJsonAsync<T>(options, token)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            return await defaultAction
                .Invoke(e)
                .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="streamTask"></param>
    /// <param name="defaultAction"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<Stream> streamTask, Func<Exception, T> defaultAction)
    {
        return await streamTask
            .DeserializeJsonAsync<T>(defaultAction, FluentHttpClientOptions.DefaultJsonSerializerOptions, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="streamTask"></param>
    /// <param name="defaultAction"></param>
    /// <param name="options"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<Stream> streamTask, Func<Exception, T> defaultAction, JsonSerializerOptions options)
    {
        return await streamTask
            .DeserializeJsonAsync<T>(defaultAction, options, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="streamTask"></param>
    /// <param name="defaultAction"></param>
    /// <param name="token"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<Stream> streamTask, Func<Exception, T> defaultAction, CancellationToken token)
    {
        return await streamTask
            .DeserializeJsonAsync<T>(defaultAction, FluentHttpClientOptions.DefaultJsonSerializerOptions, token)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="streamTask"></param>
    /// <param name="defaultAction"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<Stream> streamTask, Func<Exception, T> defaultAction, JsonSerializerOptions options, CancellationToken token)
    {
        if (defaultAction == null) throw new ArgumentNullException(nameof(defaultAction));

        try
        {
            return await streamTask
                .DeserializeJsonAsync<T>(options, token)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            return defaultAction.Invoke(e);
        }
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="streamTask"></param>
    /// <returns>A T representation of the JSON value.</returns>
    /// <exception cref="JsonException" />
    /// <exception cref="NotSupportedException" />
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<Stream> streamTask)
    {
        return await streamTask
            .DeserializeJsonAsync<T>(FluentHttpClientOptions.DefaultJsonSerializerOptions, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="streamTask"></param>
    /// <param name="options"></param>
    /// <returns>A T representation of the JSON value.</returns>
    /// <exception cref="JsonException" />
    /// <exception cref="NotSupportedException" />
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<Stream> streamTask, JsonSerializerOptions options)
    {
        return await streamTask
            .DeserializeJsonAsync<T>(options, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="streamTask"></param>
    /// <param name="token"></param>
    /// <returns>A T representation of the JSON value.</returns>
    /// <exception cref="JsonException" />
    /// <exception cref="NotSupportedException" />
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<Stream> streamTask, CancellationToken token)
    {
        return await streamTask
            .DeserializeJsonAsync<T>(FluentHttpClientOptions.DefaultJsonSerializerOptions, token)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="streamTask"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    /// <returns>A T representation of the JSON value.</returns>
    /// <exception cref="JsonException" />
    /// <exception cref="NotSupportedException" />
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Task<Stream> streamTask, JsonSerializerOptions options, CancellationToken token)
    {
        var stream = await streamTask.ConfigureAwait(false);
        return await stream
            .DeserializeJsonAsync<T>(options, token)
            .ConfigureAwait(false);
    }

    #endregion TaskStreamDeserialization

    #region StreamDeserialization

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="defaultAction"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Stream stream, Func<Exception, Task<T>> defaultAction)
    {
        return await stream
            .DeserializeJsonAsync<T>(defaultAction, FluentHttpClientOptions.DefaultJsonSerializerOptions, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="defaultAction"></param>
    /// <param name="options"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Stream stream, Func<Exception, Task<T>> defaultAction, JsonSerializerOptions options)
    {
        return await stream
            .DeserializeJsonAsync<T>(defaultAction, options, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="defaultAction"></param>
    /// <param name="token"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Stream stream, Func<Exception, Task<T>> defaultAction, CancellationToken token)
    {
        return await stream
            .DeserializeJsonAsync<T>(defaultAction, FluentHttpClientOptions.DefaultJsonSerializerOptions, token)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="defaultAction"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Stream stream, Func<Exception, Task<T>> defaultAction, JsonSerializerOptions options, CancellationToken token)
    {
        if (defaultAction == null) throw new ArgumentNullException(nameof(defaultAction));

        try
        {
            return await stream
                .DeserializeJsonAsync<T>(options, token)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            return await defaultAction
                .Invoke(e)
                .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="defaultAction"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Stream stream, Func<Exception, T> defaultAction)
    {
        return await stream
            .DeserializeJsonAsync<T>(defaultAction, FluentHttpClientOptions.DefaultJsonSerializerOptions, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="defaultAction"></param>
    /// <param name="options"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Stream stream, Func<Exception, T> defaultAction, JsonSerializerOptions options)
    {
        return await stream
            .DeserializeJsonAsync<T>(defaultAction, options, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="defaultAction"></param>
    /// <param name="token"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Stream stream, Func<Exception, T> defaultAction, CancellationToken token)
    {
        return await stream
            .DeserializeJsonAsync<T>(defaultAction, FluentHttpClientOptions.DefaultJsonSerializerOptions, token)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="defaultAction"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    /// <returns>A T representation of the JSON value or the result of the default action if an exception is thrown during the deserialization process.</returns>
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Stream stream, Func<Exception, T> defaultAction, JsonSerializerOptions options, CancellationToken token)
    {
        if (defaultAction == null) throw new ArgumentNullException(nameof(defaultAction));

        try
        {
            return await stream
                .DeserializeJsonAsync<T>(options, token)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            return defaultAction.Invoke(e);
        }
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <returns>A T representation of the JSON value.</returns>
    /// <exception cref="JsonException" />
    /// <exception cref="NotSupportedException" />
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Stream stream)
    {
        return await stream
            .DeserializeJsonAsync<T>(FluentHttpClientOptions.DefaultJsonSerializerOptions, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="options"></param>
    /// <returns>A T representation of the JSON value.</returns>
    /// <exception cref="JsonException" />
    /// <exception cref="NotSupportedException" />
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Stream stream, JsonSerializerOptions options)
    {
        return await stream
            .DeserializeJsonAsync<T>(options, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="token"></param>
    /// <returns>A T representation of the JSON value.</returns>
    /// <exception cref="JsonException" />
    /// <exception cref="NotSupportedException" />
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Stream stream, CancellationToken token)
    {
        return await stream
            .DeserializeJsonAsync<T>(FluentHttpClientOptions.DefaultJsonSerializerOptions, token)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    /// <returns>A T representation of the JSON value.</returns>
    /// <exception cref="JsonException" />
    /// <exception cref="NotSupportedException" />
    /// <exception cref="ArgumentNullException" />
    public static async Task<T?> DeserializeJsonAsync<T>(this Stream stream, JsonSerializerOptions options, CancellationToken token)
    {
        return await JsonSerializer
            .DeserializeAsync<T>(stream, options, token)
            .ConfigureAwait(false);
    }

    #endregion StreamDeserialization
}
