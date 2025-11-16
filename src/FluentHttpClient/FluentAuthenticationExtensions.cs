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
    /// <param name="builder"></param>
    /// <param name="scheme"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithAuthentication(this HttpRequestBuilder builder, string scheme, string token)
    {
        ArgumentNullException.ThrowIfNull(scheme);
        ArgumentNullException.ThrowIfNull(token);

        builder.HeaderConfigurators.Add(headers =>
        {
            headers.Authorization = new AuthenticationHeaderValue(scheme, token);
        });

        return builder;
    }

    /// <summary>
    /// Sets the authentication header to Basic using the specified token value.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithBasicAuthentication(this HttpRequestBuilder builder, string token)
    {
        ArgumentNullException.ThrowIfNull(token);

        return builder.WithAuthentication("Basic", token);
    }

    /// <summary>
    /// Sets the authentication header to Basic using the specified username and password.
    /// </summary>
    /// <remarks>
    /// The username and password will be properly concatenated and Base64 encoded.
    /// </remarks>
    /// <param name="builder"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithBasicAuthentication(this HttpRequestBuilder builder, string username, string password)
    {
        ArgumentNullException.ThrowIfNull(username);
        ArgumentNullException.ThrowIfNull(password);

        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
        return builder.WithBasicAuthentication(token);
    }

    /// <summary>
    /// Sets the authentication header to Bearer using the specified OAuth token.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static HttpRequestBuilder WithOAuthBearerToken(this HttpRequestBuilder builder, string token)
    {
        ArgumentNullException.ThrowIfNull(token);

        return builder.WithAuthentication("Bearer", token);
    }
}
