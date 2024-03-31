namespace FluentHttpClient;

/// <summary>
/// Extension methods for getting the content of an <see cref="HttpResponseMessage"/> and taking default actions based on success or failure.
/// </summary>
public static class HttpResponseMessageExtensions
{
    /// <summary>
    /// Returns the HTTP content serialized to a string as an asynchronous operation. 
    /// </summary>
    /// <param name="responseMessage"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public static async Task<string> GetResponseStringAsync(this HttpResponseMessage responseMessage)
    {
        return (responseMessage.Content != null)
            ? await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false)
            : string.Empty;
    }

    /// <summary>
    /// Returns the HTTP content serialized to a string as an asynchronous operation. 
    /// </summary>
    /// <param name="taskResponseMessage"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public static async Task<string> GetResponseStringAsync(this Task<HttpResponseMessage> taskResponseMessage)
    {
        var responseMessage = await taskResponseMessage.ConfigureAwait(false);
        return await responseMessage.GetResponseStringAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Returns the HTTP content serialized as a stream as an asynchronous operation.
    /// </summary>
    /// <param name="responseMessage"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public static async Task<Stream> GetResponseStreamAsync(this HttpResponseMessage responseMessage)
    {
        return (responseMessage.Content != null)
            ? await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false)
            : new MemoryStream();
    }

    /// <summary>
    /// Returns the HTTP content serialized as a stream as an asynchronous operation.
    /// </summary>
    /// <param name="taskResponseMessage"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public static async Task<Stream> GetResponseStreamAsync(this Task<HttpResponseMessage> taskResponseMessage)
    {
        var responseMessage = await taskResponseMessage.ConfigureAwait(false);
        return await responseMessage.GetResponseStreamAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Returns the HTTP content serialized as a byte array as an asynchronous operation.
    /// </summary>
    /// <param name="responseMessage"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public static async Task<byte[]> GetResponseBytesAsync(this HttpResponseMessage responseMessage)
    {
        return (responseMessage.Content != null)
            ? await responseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false)
            : [];
    }

    /// <summary>
    /// Returns the HTTP content serialized as a byte array as an asynchronous operation.
    /// </summary>
    /// <param name="taskResponseMessage"></param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public static async Task<byte[]> GetResponseBytesAsync(this Task<HttpResponseMessage> taskResponseMessage)
    {
        var responseMessage = await taskResponseMessage.ConfigureAwait(false);
        return await responseMessage.GetResponseBytesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Assign a delegate to execute if <see cref="HttpResponseMessage.IsSuccessStatusCode"/> is false.
    /// </summary>
    /// <param name="taskResponse"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static async Task<HttpResponseMessage> OnFailure(this Task<HttpResponseMessage> taskResponse, Action<HttpResponseMessage> action)
    {
        var response = await taskResponse.ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) action.Invoke(response);
        return response;
    }

    /// <summary>
    /// Assign an async delegate to execute if <see cref="HttpResponseMessage.IsSuccessStatusCode"/> is false.
    /// </summary>
    /// <param name="taskResponse"></param>
    /// <param name="function"></param>
    /// <returns></returns>
    public static async Task<HttpResponseMessage> OnFailureAsync(this Task<HttpResponseMessage> taskResponse, Func<HttpResponseMessage, Task> function)
    {
        var response = await taskResponse.ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) await function.Invoke(response).ConfigureAwait(false);
        return response;
    }

    /// <summary>
    /// Assign a delegate to execute if <see cref="HttpResponseMessage.IsSuccessStatusCode"/> is true.
    /// </summary>
    /// <param name="taskResponse"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static async Task<HttpResponseMessage> OnSuccess(this Task<HttpResponseMessage> taskResponse, Action<HttpResponseMessage> action)
    {
        var response = await taskResponse.ConfigureAwait(false);
        if (response.IsSuccessStatusCode) action.Invoke(response);
        return response;
    }

    /// <summary>
    /// Assign an async delegate to execute if <see cref="HttpResponseMessage.IsSuccessStatusCode"/> is true.
    /// </summary>
    /// <param name="taskResponse"></param>
    /// <param name="function"></param>
    /// <returns></returns>
    public static async Task<HttpResponseMessage> OnSuccessAsync(this Task<HttpResponseMessage> taskResponse, Func<HttpResponseMessage, Task> function)
    {
        var response = await taskResponse.ConfigureAwait(false);
        if (response.IsSuccessStatusCode) await function.Invoke(response).ConfigureAwait(false);
        return response;
    }
}