namespace FluentHttpClient;

/// <summary>
/// Fluent extension methods for sending HTTP requests using specific HTTP methods
/// on an <see cref="HttpRequestBuilder"/> instance.
/// </summary>
public static class FluentSendExtensions
{
    // DELETE

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
        return builder.SendAsync(HttpMethod.Delete, cancellationToken: cancellationToken);
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

    // GET

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
        return builder.SendAsync(HttpMethod.Get, cancellationToken: cancellationToken);
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

    // HEAD

    /// <summary>
    /// Sends an HTTP HEAD request using the configured <see cref="HttpRequestBuilder"/>.
    /// </summary>
    /// <param name="builder"></param>
    public static Task<HttpResponseMessage> HeadAsync(this HttpRequestBuilder builder)
    {
        return builder.SendAsync(HttpMethod.Head);
    }

    /// <summary>
    /// Sends an HTTP HEAD request using the specified <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="cancellationToken"></param>
    public static Task<HttpResponseMessage> HeadAsync(
        this HttpRequestBuilder builder,
        CancellationToken cancellationToken)
    {
        return builder.SendAsync(HttpMethod.Head, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP HEAD request using the specified <see cref="HttpCompletionOption"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="completionOption"></param>
    public static Task<HttpResponseMessage> HeadAsync(
        this HttpRequestBuilder builder,
        HttpCompletionOption completionOption)
    {
        return builder.SendAsync(HttpMethod.Head, completionOption);
    }

    /// <summary>
    /// Sends an HTTP HEAD request using the specified <see cref="HttpCompletionOption"/> and <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="completionOption"></param>
    /// <param name="cancellationToken"></param>
    public static Task<HttpResponseMessage> HeadAsync(
        this HttpRequestBuilder builder,
        HttpCompletionOption completionOption,
        CancellationToken cancellationToken)
    {
        return builder.SendAsync(HttpMethod.Head, completionOption, cancellationToken);
    }

    // OPTIONS

    /// <summary>
    /// Sends an HTTP OPTIONS request using the configured <see cref="HttpRequestBuilder"/>.
    /// </summary>
    /// <param name="builder"></param>
    public static Task<HttpResponseMessage> OptionsAsync(this HttpRequestBuilder builder)
    {
        return builder.SendAsync(HttpMethod.Options);
    }

    /// <summary>
    /// Sends an HTTP OPTIONS request using the specified <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="cancellationToken"></param>
    public static Task<HttpResponseMessage> OptionsAsync(
        this HttpRequestBuilder builder,
        CancellationToken cancellationToken)
    {
        return builder.SendAsync(HttpMethod.Options, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP OPTIONS request using the specified <see cref="HttpCompletionOption"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="completionOption"></param>
    public static Task<HttpResponseMessage> OptionsAsync(
        this HttpRequestBuilder builder,
        HttpCompletionOption completionOption)
    {
        return builder.SendAsync(HttpMethod.Options, completionOption);
    }

    /// <summary>
    /// Sends an HTTP OPTIONS request using the specified <see cref="HttpCompletionOption"/> and <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="completionOption"></param>
    /// <param name="cancellationToken"></param>
    public static Task<HttpResponseMessage> OptionsAsync(
        this HttpRequestBuilder builder,
        HttpCompletionOption completionOption,
        CancellationToken cancellationToken)
    {
        return builder.SendAsync(HttpMethod.Options, completionOption, cancellationToken);
    }

    // PATCH

    /// <summary>
    /// Sends an HTTP PATCH request using the configured <see cref="HttpRequestBuilder"/>.
    /// </summary>
    /// <param name="builder"></param>
    public static Task<HttpResponseMessage> PatchAsync(this HttpRequestBuilder builder)
    {
#if NETSTANDARD2_0
        return builder.SendAsync(new HttpMethod("PATCH"));
#else
        return builder.SendAsync(HttpMethod.Patch);
#endif
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
#if NETSTANDARD2_0
        return builder.SendAsync(new HttpMethod("PATCH"), cancellationToken: cancellationToken);
#else
        return builder.SendAsync(HttpMethod.Patch, cancellationToken: cancellationToken);
#endif
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
#if NETSTANDARD2_0
        return builder.SendAsync(new HttpMethod("PATCH"), completionOption);
#else
        return builder.SendAsync(HttpMethod.Patch, completionOption);
#endif
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
#if NETSTANDARD2_0
        return builder.SendAsync(new HttpMethod("PATCH"), completionOption, cancellationToken);
#else
        return builder.SendAsync(HttpMethod.Patch, completionOption, cancellationToken);
#endif
    }

    // POST

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
        return builder.SendAsync(HttpMethod.Post, cancellationToken: cancellationToken);
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

    // PUT

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
        return builder.SendAsync(HttpMethod.Put, cancellationToken: cancellationToken);
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
}
