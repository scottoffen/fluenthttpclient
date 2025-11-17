namespace FluentHttpClient;

/// <summary>
/// Fluent extension methods for performing actions based on a predicate on an <see cref="HttpResponseMessage"/> instance.
/// </summary>
public static class FluentResponseHandlerExtensions
{
    /// <summary>
    /// Executes the specified handler if the predicate returns true for the HTTP response.
    /// </summary>
    /// <param name="taskResponse">
    /// The pending HTTP response task that will be inspected when it completes.
    /// </param>
    /// <param name="predicate">
    /// A function that evaluates the <see cref="HttpResponseMessage"/> and determines
    /// whether the handler should be invoked.
    /// </param>
    /// <param name="handler">
    /// The action to execute when the predicate returns true.
    /// </param>
    /// <returns>
    /// A task that completes with the original <see cref="HttpResponseMessage"/> instance
    /// after the handler has run, if applicable.
    /// </returns>
    /// <remarks>
    /// This method does not dispose the <see cref="HttpResponseMessage"/>. The caller
    /// remains responsible for disposal. If the handler reads or consumes the response
    /// content, subsequent chained methods may no longer be able to read the content.
    /// </remarks>
    public static async Task<HttpResponseMessage> When(
        this Task<HttpResponseMessage> taskResponse,
        Func<HttpResponseMessage, bool> predicate,
        Action<HttpResponseMessage> handler)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(handler);

        var response = await taskResponse.ConfigureAwait(false);

        if (predicate(response))
        {
            handler(response);
        }

        return response;
    }

    /// <summary>
    /// Executes the specified asynchronous handler if the predicate returns true for the HTTP response.
    /// </summary>
    /// <param name="taskResponse">
    /// The pending HTTP response task that will be inspected when it completes.
    /// </param>
    /// <param name="predicate">
    /// A function that evaluates the <see cref="HttpResponseMessage"/> and determines
    /// whether the handler should be invoked.
    /// </param>
    /// <param name="handler">
    /// The asynchronous function to execute when the predicate returns true.
    /// </param>
    /// <returns>
    /// A task that completes with the original <see cref="HttpResponseMessage"/> instance
    /// after the handler has run, if applicable.
    /// </returns>
    /// <remarks>
    /// This method does not dispose the <see cref="HttpResponseMessage"/>. The caller
    /// remains responsible for disposal. If the handler reads or consumes the response
    /// content, subsequent chained methods may no longer be able to read the content.
    /// </remarks>
    public static async Task<HttpResponseMessage> When(
        this Task<HttpResponseMessage> taskResponse,
        Func<HttpResponseMessage, bool> predicate,
        Func<HttpResponseMessage, Task> handler)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(handler);

        var response = await taskResponse.ConfigureAwait(false);

        if (predicate(response))
        {
            await handler(response).ConfigureAwait(false);
        }

        return response;
    }

    /// <summary>
    /// Executes the specified handler if <see cref="HttpResponseMessage.IsSuccessStatusCode"/> is true.
    /// </summary>
    /// <param name="taskResponse">The pending HTTP response task.</param>
    /// <param name="handler">The action to invoke when the response indicates success.</param>
    /// <returns>
    /// A task that concludes with the original <see cref="HttpResponseMessage"/> after the handler has run, if applicable.
    /// </returns>
    public static Task<HttpResponseMessage> OnSuccess(
        this Task<HttpResponseMessage> taskResponse,
        Action<HttpResponseMessage> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        return taskResponse.When(r => r.IsSuccessStatusCode, handler);
    }

    /// <summary>
    /// Executes the specified asynchronous handler if <see cref="HttpResponseMessage.IsSuccessStatusCode"/> is true.
    /// </summary>
    /// <param name="taskResponse">The pending HTTP response task.</param>
    /// <param name="handler">The asynchronous function to invoke when the response indicates success.</param>
    /// <returns>
    /// A task that concludes with the original <see cref="HttpResponseMessage"/> after the handler has run, if applicable.
    /// </returns>
    public static Task<HttpResponseMessage> OnSuccess(
        this Task<HttpResponseMessage> taskResponse,
        Func<HttpResponseMessage, Task> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        return taskResponse.When(r => r.IsSuccessStatusCode, handler);
    }

    /// <summary>
    /// Executes the specified handler if <see cref="HttpResponseMessage.IsSuccessStatusCode"/> is false.
    /// </summary>
    /// <param name="taskResponse">The pending HTTP response task.</param>
    /// <param name="handler">The action to invoke when the response indicates failure.</param>
    /// <returns>
    /// A task that concludes with the original <see cref="HttpResponseMessage"/> after the handler has run, if applicable.
    /// </returns>
    public static Task<HttpResponseMessage> OnFailure(
        this Task<HttpResponseMessage> taskResponse,
        Action<HttpResponseMessage> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        return taskResponse.When(r => !r.IsSuccessStatusCode, handler);
    }

    /// <summary>
    /// Executes the specified asynchronous handler if <see cref="HttpResponseMessage.IsSuccessStatusCode"/> is false.
    /// </summary>
    /// <param name="taskResponse">The pending HTTP response task.</param>
    /// <param name="handler">The asynchronous function to invoke when the response indicates failure.</param>
    /// <returns>
    /// A task that concludes with the original <see cref="HttpResponseMessage"/> after the handler has run, if applicable.
    /// </returns>
    public static Task<HttpResponseMessage> OnFailure(
        this Task<HttpResponseMessage> taskResponse,
        Func<HttpResponseMessage, Task> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        return taskResponse.When(r => !r.IsSuccessStatusCode, handler);
    }
}
