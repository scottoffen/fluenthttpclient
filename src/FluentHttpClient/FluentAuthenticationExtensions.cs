using System.Net.Http.Headers;
using System.Text;

namespace FluentHttpClient;

/// <summary>
/// Fluent extension methods for configuring authentication headers on an <see cref="HttpRequestBuilder"/> instance.
/// </summary>
public static class FluentAuthenticationExtensions
{
    /// <summary>
    /// Sets the <see cref="AuthenticationHeaderValue"/> for the request using the specified scheme and token.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="scheme">The authentication scheme (e.g., "Bearer", "Basic").</param>
    /// <param name="token">The authentication token or credentials.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithAuthentication(this HttpRequestBuilder builder, string scheme, string token)
    {
        Guard.AgainstNull(scheme, nameof(scheme));
        Guard.AgainstNull(token, nameof(token));

        builder.HeaderConfigurators.Add(headers =>
        {
            headers.Authorization = new AuthenticationHeaderValue(scheme, token);
        });

        return builder;
    }

    /// <summary>
    /// Sets the authentication header to Basic using the specified token value.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="token">The Base64-encoded Basic authentication token.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithBasicAuthentication(this HttpRequestBuilder builder, string token)
    {
        Guard.AgainstNull(token, nameof(token));

        return builder.WithAuthentication("Basic", token);
    }

    /// <summary>
    /// Sets the authentication header to Basic using the specified username and password.
    /// </summary>
    /// <remarks>
    /// The username and password will be properly concatenated and Base64 encoded.
    /// </remarks>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="username">The username for Basic authentication.</param>
    /// <param name="password">The password for Basic authentication.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithBasicAuthentication(this HttpRequestBuilder builder, string username, string password)
    {
        Guard.AgainstNull(username, nameof(username));
        Guard.AgainstNull(password, nameof(password));

        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
        return builder.WithBasicAuthentication(token);
    }

    /// <summary>
    /// Sets the authentication header to Bearer using the specified OAuth token.
    /// </summary>
    /// <param name="builder">The <see cref="HttpRequestBuilder"/> instance.</param>
    /// <param name="token">The OAuth Bearer token.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for method chaining.</returns>
    public static HttpRequestBuilder WithOAuthBearerToken(this HttpRequestBuilder builder, string token)
    {
        Guard.AgainstNull(token, nameof(token));

        return builder.WithAuthentication("Bearer", token);
    }
}
