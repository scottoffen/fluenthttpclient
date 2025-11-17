using System.Diagnostics.CodeAnalysis;

namespace FluentHttpClient;

/// <summary>
/// Fluent extension methods for reading HTTP response content from
/// <see cref="HttpResponseMessage"/> and <see cref="Task{HttpResponseMessage}"/> instances.
/// </summary>
public static class FluentResponseContentExtensions
{
    /// <summary>
    /// Reads the HTTP response content as a string.
    /// </summary>
    /// <remarks>
    /// This method consumes the underlying response content stream. If the response content
    /// is not buffered by the underlying handler, it can only be read once. Callers that
    /// need to read the content multiple times should buffer it manually, for example by
    /// reading it into a byte array first.
    /// </remarks>
    /// <param name="responseMessage">The HTTP response whose content will be read.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the
    /// response content as a string, or <see cref="string.Empty"/> if there is no content.
    /// </returns>
    public static Task<string> ReadContentAsStringAsync(this HttpResponseMessage responseMessage) =>
        responseMessage.ReadContentAsStringAsync(CancellationToken.None);

    /// <summary>
    /// Reads the HTTP response content as a string, observing a cancellation token.
    /// </summary>
    /// <remarks>
    /// This method consumes the underlying response content stream. If the response content
    /// is not buffered by the underlying handler, it can only be read once. Callers that
    /// need to read the content multiple times should buffer it manually, for example by
    /// reading it into a byte array first.
    /// <para>
    /// The <paramref name="cancellationToken"/> only applies to reading the response content.
    /// It does not cancel the original HTTP request, which must be configured separately
    /// on the send operation.
    /// </para>
    /// </remarks>
    /// <param name="responseMessage">The HTTP response whose content will be read.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the
    /// response content as a string, or <see cref="string.Empty"/> if there is no content.
    /// </returns>
    public static async Task<string> ReadContentAsStringAsync(
        this HttpResponseMessage responseMessage,
        CancellationToken cancellationToken)
    {
        // NOTE: HttpResponseMessage.Content is never null on modern TFMs, but can be on older platforms.
        // These checks exist for cross-target safety and are not hit in current test runs.
        if (responseMessage.Content is null)
        {
            return string.Empty;
        }

#if NET5_0_OR_GREATER
        return await responseMessage.Content
            .ReadAsStringAsync(cancellationToken)
            .ConfigureAwait(false);
#else
        cancellationToken.ThrowIfCancellationRequested();

        return await responseMessage.Content
            .ReadAsStringAsync()
            .ConfigureAwait(false);
#endif
    }

    /// <summary>
    /// Reads the HTTP response content as a string from a task that produces a response.
    /// </summary>
    /// <remarks>
    /// The supplied <see cref="Task{HttpResponseMessage}"/> is awaited to obtain the response,
    /// and then its content is read. This method consumes the underlying response content
    /// stream, and repeat reads may not be possible unless the content is buffered.
    /// </remarks>
    /// <param name="responseTask">A task that produces the HTTP response whose content will be read.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the
    /// response content as a string, or <see cref="string.Empty"/> if there is no content.
    /// </returns>
    public static Task<string> ReadContentAsStringAsync(this Task<HttpResponseMessage> responseTask) =>
        responseTask.ReadContentAsStringAsync(CancellationToken.None);

    /// <summary>
    /// Reads the HTTP response content as a string from a task that produces a response,
    /// observing a cancellation token.
    /// </summary>
    /// <remarks>
    /// The supplied <see cref="Task{HttpResponseMessage}"/> is awaited to obtain the response,
    /// and then its content is read. This method consumes the underlying response content
    /// stream, and repeat reads may not be possible unless the content is buffered.
    /// <para>
    /// The <paramref name="cancellationToken"/> only applies to reading the response content.
    /// It does not cancel the original HTTP request, which must be configured separately
    /// on the send operation.
    /// </para>
    /// </remarks>
    /// <param name="responseTask">A task that produces the HTTP response whose content will be read.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the
    /// response content as a string, or <see cref="string.Empty"/> if there is no content.
    /// </returns>
    public static async Task<string> ReadContentAsStringAsync(
        this Task<HttpResponseMessage> responseTask,
        CancellationToken cancellationToken)
    {
        var response = await responseTask.ConfigureAwait(false);
        return await response.ReadContentAsStringAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Reads the HTTP response content as a stream.
    /// </summary>
    /// <remarks>
    /// This method consumes the underlying response content stream. If the response content
    /// is not buffered by the underlying handler, it can only be read once. Callers that
    /// need to read the content multiple times should buffer it manually.
    /// <para>
    /// When there is no content, this method returns <see cref="Stream.Null"/>.
    /// </para>
    /// </remarks>
    /// <param name="responseMessage">The HTTP response whose content will be read.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the
    /// response content stream, or <see cref="Stream.Null"/> if there is no content.
    /// </returns>
    public static Task<Stream> ReadContentAsStreamAsync(this HttpResponseMessage responseMessage) =>
        responseMessage.ReadContentAsStreamAsync(CancellationToken.None);

    /// <summary>
    /// Reads the HTTP response content as a stream, observing a cancellation token.
    /// </summary>
    /// <remarks>
    /// This method consumes the underlying response content stream. If the response content
    /// is not buffered by the underlying handler, it can only be read once. Callers that
    /// need to read the content multiple times should buffer it manually.
    /// <para>
    /// When there is no content, this method returns <see cref="Stream.Null"/>.
    /// </para>
    /// <para>
    /// The <paramref name="cancellationToken"/> only applies to reading the response content.
    /// It does not cancel the original HTTP request, which must be configured separately
    /// on the send operation.
    /// </para>
    /// </remarks>
    /// <param name="responseMessage">The HTTP response whose content will be read.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the
    /// response content stream, or <see cref="Stream.Null"/> if there is no content.
    /// </returns>
    public static async Task<Stream> ReadContentAsStreamAsync(
        this HttpResponseMessage responseMessage,
        CancellationToken cancellationToken)
    {

        // NOTE: HttpResponseMessage.Content is never null on modern TFMs, but can be on older platforms.
        // These checks exist for cross-target safety and are not hit in current test runs.
        if (responseMessage.Content is null)
        {
            return Stream.Null;
        }

#if NET5_0_OR_GREATER
        return await responseMessage.Content
            .ReadAsStreamAsync(cancellationToken)
            .ConfigureAwait(false);
#else
        cancellationToken.ThrowIfCancellationRequested();

        return await responseMessage.Content
            .ReadAsStreamAsync()
            .ConfigureAwait(false);
#endif
    }

    /// <summary>
    /// Reads the HTTP response content as a stream from a task that produces a response.
    /// </summary>
    /// <remarks>
    /// The supplied <see cref="Task{HttpResponseMessage}"/> is awaited to obtain the response,
    /// and then its content is read. This method consumes the underlying response content
    /// stream, and repeat reads may not be possible unless the content is buffered.
    /// </remarks>
    /// <param name="responseTask">A task that produces the HTTP response whose content will be read.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the
    /// response content stream, or <see cref="Stream.Null"/> if there is no content.
    /// </returns>
    public static Task<Stream> ReadContentAsStreamAsync(this Task<HttpResponseMessage> responseTask) =>
        responseTask.ReadContentAsStreamAsync(CancellationToken.None);

    /// <summary>
    /// Reads the HTTP response content as a stream from a task that produces a response,
    /// observing a cancellation token.
    /// </summary>
    /// <remarks>
    /// The supplied <see cref="Task{HttpResponseMessage}"/> is awaited to obtain the response,
    /// and then its content is read. This method consumes the underlying response content
    /// stream, and repeat reads may not be possible unless the content is buffered.
    /// <para>
    /// The <paramref name="cancellationToken"/> only applies to reading the response content.
    /// It does not cancel the original HTTP request, which must be configured separately
    /// on the send operation.
    /// </para>
    /// </remarks>
    /// <param name="responseTask">A task that produces the HTTP response whose content will be read.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the
    /// response content stream, or <see cref="Stream.Null"/> if there is no content.
    /// </returns>
    public static async Task<Stream> ReadContentAsStreamAsync(
        this Task<HttpResponseMessage> responseTask,
        CancellationToken cancellationToken)
    {
        var response = await responseTask.ConfigureAwait(false);
        return await response.ReadContentAsStreamAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Reads the HTTP response content as a byte array.
    /// </summary>
    /// <remarks>
    /// This method consumes the underlying response content stream. If the response content
    /// is not buffered by the underlying handler, it can only be read once. Callers that
    /// need to read the content multiple times should retain the returned byte array.
    /// <para>
    /// When there is no content, this method returns an empty byte array.
    /// </para>
    /// </remarks>
    /// <param name="responseMessage">The HTTP response whose content will be read.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the
    /// response content as a byte array, or an empty array if there is no content.
    /// </returns>
    public static Task<byte[]> ReadContentAsByteArrayAsync(this HttpResponseMessage responseMessage) =>
        responseMessage.ReadContentAsByteArrayAsync(CancellationToken.None);

    /// <summary>
    /// Reads the HTTP response content as a byte array, observing a cancellation token.
    /// </summary>
    /// <remarks>
    /// This method consumes the underlying response content stream. If the response content
    /// is not buffered by the underlying handler, it can only be read once. Callers that
    /// need to read the content multiple times should retain the returned byte array.
    /// <para>
    /// When there is no content, this method returns an empty byte array.
    /// </para>
    /// <para>
    /// The <paramref name="cancellationToken"/> only applies to reading the response content.
    /// It does not cancel the original HTTP request, which must be configured separately
    /// on the send operation.
    /// </para>
    /// </remarks>
    /// <param name="responseMessage">The HTTP response whose content will be read.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the
    /// response content as a byte array, or an empty array if there is no content.
    /// </returns>
    public static async Task<byte[]> ReadContentAsByteArrayAsync(
        this HttpResponseMessage responseMessage,
        CancellationToken cancellationToken)
    {
        // NOTE: HttpResponseMessage.Content is never null on modern TFMs, but can be on older platforms.
        // These checks exist for cross-target safety and are not hit in current test runs.
        if (responseMessage.Content is null)
        {
            return [];
        }

#if NET5_0_OR_GREATER
        return await responseMessage.Content
            .ReadAsByteArrayAsync(cancellationToken)
            .ConfigureAwait(false);
#else
        cancellationToken.ThrowIfCancellationRequested();

        return await responseMessage.Content
            .ReadAsByteArrayAsync()
            .ConfigureAwait(false);
#endif
    }

    /// <summary>
    /// Reads the HTTP response content as a byte array from a task that produces a response.
    /// </summary>
    /// <remarks>
    /// The supplied <see cref="Task{HttpResponseMessage}"/> is awaited to obtain the response,
    /// and then its content is read. This method consumes the underlying response content
    /// stream, and repeat reads may not be possible unless the content is buffered.
    /// </remarks>
    /// <param name="responseTask">A task that produces the HTTP response whose content will be read.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the
    /// response content as a byte array, or an empty array if there is no content.
    /// </returns>
    public static Task<byte[]> ReadContentAsByteArrayAsync(this Task<HttpResponseMessage> responseTask) =>
        responseTask.ReadContentAsByteArrayAsync(CancellationToken.None);

    /// <summary>
    /// Reads the HTTP response content as a byte array from a task that produces a response,
    /// observing a cancellation token.
    /// </summary>
    /// <remarks>
    /// The supplied <see cref="Task{HttpResponseMessage}"/> is awaited to obtain the response,
    /// and then its content is read. This method consumes the underlying response content
    /// stream, and repeat reads may not be possible unless the content is buffered.
    /// <para>
    /// The <paramref name="cancellationToken"/> only applies to reading the response content.
    /// It does not cancel the original HTTP request, which must be configured separately
    /// on the send operation.
    /// </para>
    /// </remarks>
    /// <param name="responseTask">A task that produces the HTTP response whose content will be read.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the
    /// response content as a byte array, or an empty array if there is no content.
    /// </returns>
    public static async Task<byte[]> ReadContentAsByteArrayAsync(
        this Task<HttpResponseMessage> responseTask,
        CancellationToken cancellationToken)
    {
        var response = await responseTask.ConfigureAwait(false);
        return await response.ReadContentAsByteArrayAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    // Compatibility shims for older method names.
    // Mark them as error:true in future 5.x release, and remove them in v6.0.0

    /// <inheritdoc cref="ReadContentAsStringAsync(HttpResponseMessage)"/>
    [Obsolete("Use ReadContentAsStringAsync instead.")]
    [ExcludeFromCodeCoverage]
    public static Task<string> GetResponseStringAsync(this HttpResponseMessage responseMessage) =>
        responseMessage.ReadContentAsStringAsync();

    /// <inheritdoc cref="ReadContentAsStringAsync(Task{HttpResponseMessage})"/>
    [Obsolete("Use ReadContentAsStringAsync instead.")]
    [ExcludeFromCodeCoverage]
    public static Task<string> GetResponseStringAsync(this Task<HttpResponseMessage> responseTask) =>
        responseTask.ReadContentAsStringAsync();

    /// <inheritdoc cref="ReadContentAsStreamAsync(HttpResponseMessage)"/>
    [Obsolete("Use ReadContentAsStreamAsync instead.")]
    [ExcludeFromCodeCoverage]
    public static Task<Stream> GetResponseStreamAsync(this HttpResponseMessage responseMessage) =>
        responseMessage.ReadContentAsStreamAsync();

    /// <inheritdoc cref="ReadContentAsStreamAsync(Task{HttpResponseMessage})"/>
    [Obsolete("Use ReadContentAsStreamAsync instead.")]
    [ExcludeFromCodeCoverage]
    public static Task<Stream> GetResponseStreamAsync(this Task<HttpResponseMessage> responseTask) =>
        responseTask.ReadContentAsStreamAsync();

    /// <inheritdoc cref="ReadContentAsByteArrayAsync(HttpResponseMessage)"/>
    [Obsolete("Use ReadContentAsByteArrayAsync instead.")]
    [ExcludeFromCodeCoverage]
    public static Task<byte[]> GetResponseBytesAsync(this HttpResponseMessage responseMessage) =>
        responseMessage.ReadContentAsByteArrayAsync();

    /// <inheritdoc cref="ReadContentAsByteArrayAsync(Task{HttpResponseMessage})"/>
    [Obsolete("Use ReadContentAsByteArrayAsync instead.")]
    [ExcludeFromCodeCoverage]
    public static Task<byte[]> GetResponseBytesAsync(this Task<HttpResponseMessage> responseTask) =>
        responseTask.ReadContentAsByteArrayAsync();
}
