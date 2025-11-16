namespace FluentHttpClient;

/// <summary>
/// Fluent extension methods for sending HTTP requests using specific HTTP methods
/// on an <see cref="HttpRequestBuilder"/> instance.
/// </summary>
public static class FluentSendExtensions
{
    #region DELETE

    /// <summary>
    /// Sends an HTTP DELETE request using the configured <see cref="HttpRequestBuilder"/>.
    /// </summary>
    /// <param name="builder"></param>
    public static Task<HttpResponseMessage> DeleteAsync(this HttpRequestBuilder builder)
    {
        return builder.SendAsync(HttpMethod.Delete);
    }

    /// <summary>
    /// Sends an HTTP DELETE request using the specified <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="cancellationToken"></param>
    public static Task<HttpResponseMessage> DeleteAsync(
        this HttpRequestBuilder builder,
        CancellationToken cancellationToken)
    {
        return builder.SendAsync(HttpMethod.Delete, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP DELETE request using the specified <see cref="HttpCompletionOption"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="completionOption"></param>
    public static Task<HttpResponseMessage> DeleteAsync(
        this HttpRequestBuilder builder,
        HttpCompletionOption completionOption)
    {
        return builder.SendAsync(HttpMethod.Delete, completionOption);
    }

    /// <summary>
    /// Sends an HTTP DELETE request using the specified <see cref="HttpCompletionOption"/> and <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="completionOption"></param>
    /// <param name="cancellationToken"></param>
    public static Task<HttpResponseMessage> DeleteAsync(
        this HttpRequestBuilder builder,
        HttpCompletionOption completionOption,
        CancellationToken cancellationToken)
    {
        return builder.SendAsync(HttpMethod.Delete, completionOption, cancellationToken);
    }

    #endregion

    #region GET

    /// <summary>
    /// Sends an HTTP GET request using the configured <see cref="HttpRequestBuilder"/>.
    /// </summary>
    /// <param name="builder"></param>
    public static Task<HttpResponseMessage> GetAsync(this HttpRequestBuilder builder)
    {
        return builder.SendAsync(HttpMethod.Get);
    }

    /// <summary>
    /// Sends an HTTP GET request using the specified <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="cancellationToken"></param>
    public static Task<HttpResponseMessage> GetAsync(
        this HttpRequestBuilder builder,
        CancellationToken cancellationToken)
    {
        return builder.SendAsync(HttpMethod.Get, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP GET request using the specified <see cref="HttpCompletionOption"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="completionOption"></param>
    public static Task<HttpResponseMessage> GetAsync(
        this HttpRequestBuilder builder,
        HttpCompletionOption completionOption)
    {
        return builder.SendAsync(HttpMethod.Get, completionOption);
    }

    /// <summary>
    /// Sends an HTTP GET request using the specified <see cref="HttpCompletionOption"/> and <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="completionOption"></param>
    /// <param name="cancellationToken"></param>
    public static Task<HttpResponseMessage> GetAsync(
        this HttpRequestBuilder builder,
        HttpCompletionOption completionOption,
        CancellationToken cancellationToken)
    {
        return builder.SendAsync(HttpMethod.Get, completionOption, cancellationToken);
    }

    #endregion

    #region PATCH

    /// <summary>
    /// Sends an HTTP PATCH request using the configured <see cref="HttpRequestBuilder"/>.
    /// </summary>
    /// <param name="builder"></param>
    public static Task<HttpResponseMessage> PatchAsync(this HttpRequestBuilder builder)
    {
        return builder.SendAsync(HttpMethod.Patch);
    }

    /// <summary>
    /// Sends an HTTP PATCH request using the specified <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="cancellationToken"></param>
    public static Task<HttpResponseMessage> PatchAsync(
        this HttpRequestBuilder builder,
        CancellationToken cancellationToken)
    {
        return builder.SendAsync(HttpMethod.Patch, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP PATCH request using the specified <see cref="HttpCompletionOption"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="completionOption"></param>
    public static Task<HttpResponseMessage> PatchAsync(
        this HttpRequestBuilder builder,
        HttpCompletionOption completionOption)
    {
        return builder.SendAsync(HttpMethod.Patch, completionOption);
    }

    /// <summary>
    /// Sends an HTTP PATCH request using the specified <see cref="HttpCompletionOption"/> and <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="completionOption"></param>
    /// <param name="cancellationToken"></param>
    public static Task<HttpResponseMessage> PatchAsync(
        this HttpRequestBuilder builder,
        HttpCompletionOption completionOption,
        CancellationToken cancellationToken)
    {
        return builder.SendAsync(HttpMethod.Patch, completionOption, cancellationToken);
    }

    #endregion

    #region POST

    /// <summary>
    /// Sends an HTTP POST request using the configured <see cref="HttpRequestBuilder"/>.
    /// </summary>
    /// <param name="builder"></param>
    public static Task<HttpResponseMessage> PostAsync(this HttpRequestBuilder builder)
    {
        return builder.SendAsync(HttpMethod.Post);
    }

    /// <summary>
    /// Sends an HTTP POST request using the specified <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="cancellationToken"></param>
    public static Task<HttpResponseMessage> PostAsync(
        this HttpRequestBuilder builder,
        CancellationToken cancellationToken)
    {
        return builder.SendAsync(HttpMethod.Post, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP POST request using the specified <see cref="HttpCompletionOption"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="completionOption"></param>
    public static Task<HttpResponseMessage> PostAsync(
        this HttpRequestBuilder builder,
        HttpCompletionOption completionOption)
    {
        return builder.SendAsync(HttpMethod.Post, completionOption);
    }

    /// <summary>
    /// Sends an HTTP POST request using the specified <see cref="HttpCompletionOption"/> and <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="completionOption"></param>
    /// <param name="cancellationToken"></param>
    public static Task<HttpResponseMessage> PostAsync(
        this HttpRequestBuilder builder,
        HttpCompletionOption completionOption,
        CancellationToken cancellationToken)
    {
        return builder.SendAsync(HttpMethod.Post, completionOption, cancellationToken);
    }

    #endregion

    #region PUT

    /// <summary>
    /// Sends an HTTP PUT request using the configured <see cref="HttpRequestBuilder"/>.
    /// </summary>
    /// <param name="builder"></param>
    public static Task<HttpResponseMessage> PutAsync(this HttpRequestBuilder builder)
    {
        return builder.SendAsync(HttpMethod.Put);
    }

    /// <summary>
    /// Sends an HTTP PUT request using the specified <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="cancellationToken"></param>
    public static Task<HttpResponseMessage> PutAsync(
        this HttpRequestBuilder builder,
        CancellationToken cancellationToken)
    {
        return builder.SendAsync(HttpMethod.Put, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP PUT request using the specified <see cref="HttpCompletionOption"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="completionOption"></param>
    public static Task<HttpResponseMessage> PutAsync(
        this HttpRequestBuilder builder,
        HttpCompletionOption completionOption)
    {
        return builder.SendAsync(HttpMethod.Put, completionOption);
    }

    /// <summary>
    /// Sends an HTTP PUT request using the specified <see cref="HttpCompletionOption"/> and <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="completionOption"></param>
    /// <param name="cancellationToken"></param>
    public static Task<HttpResponseMessage> PutAsync(
        this HttpRequestBuilder builder,
        HttpCompletionOption completionOption,
        CancellationToken cancellationToken)
    {
        return builder.SendAsync(HttpMethod.Put, completionOption, cancellationToken);
    }

    #endregion
}
