using System.Net.Http.Headers;
using System.Text;

namespace FluentHttpClient.Tests;

public class FluentAuthenticationExtensionsTests
{
    private static HttpRequestBuilder CreateBuilder()
    {
        var client = new HttpClient();
        return new HttpRequestBuilder(client, "https://example.com/");
    }

    private static HttpRequestHeaders ApplyHeaderConfigurators(HttpRequestBuilder builder)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/");

        foreach (var configurator in builder.HeaderConfigurators)
        {
            configurator(request.Headers);
        }

        return request.Headers;
    }

    [Fact]
    public void WithAuthentication_SetsAuthorizationHeader_WhenCalled()
    {
        var builder = CreateBuilder();

        var result = builder.WithAuthentication("CustomScheme", "token-value");

        result.ShouldBeSameAs(builder);

        var headers = ApplyHeaderConfigurators(builder);

        headers.Authorization.ShouldNotBeNull();
        headers.Authorization!.Scheme.ShouldBe("CustomScheme");
        headers.Authorization.Parameter.ShouldBe("token-value");
    }

    [Fact]
    public void WithAuthentication_ThrowsArgumentNullException_WhenSchemeIsNull()
    {
        var builder = CreateBuilder();

        var exception = Should.Throw<ArgumentNullException>(() =>
            builder.WithAuthentication(null!, "token"));

        exception.ParamName.ShouldBe("scheme");
    }

    [Fact]
    public void WithAuthentication_ThrowsArgumentNullException_WhenTokenIsNull()
    {
        var builder = CreateBuilder();

        var exception = Should.Throw<ArgumentNullException>(() =>
            builder.WithAuthentication("CustomScheme", null!));

        exception.ParamName.ShouldBe("token");
    }

    [Fact]
    public void WithBasicAuthentication_UsesBasicScheme_WhenTokenProvided()
    {
        var builder = CreateBuilder();
        var token = "raw-basic-token";

        builder.WithBasicAuthentication(token);

        var headers = ApplyHeaderConfigurators(builder);

        headers.Authorization.ShouldNotBeNull();
        headers.Authorization!.Scheme.ShouldBe("Basic");
        headers.Authorization.Parameter.ShouldBe(token);
    }

    [Fact]
    public void WithBasicAuthentication_ThrowsArgumentNullException_WhenTokenIsNull()
    {
        var builder = CreateBuilder();

        var exception = Should.Throw<ArgumentNullException>(() =>
            builder.WithBasicAuthentication(null!));

        exception.ParamName.ShouldBe("token");
    }

    [Fact]
    public void WithBasicAuthentication_EncodesUsernameAndPassword_WhenCredentialsProvided()
    {
        var builder = CreateBuilder();
        var username = "user";
        var password = "pass";

        builder.WithBasicAuthentication(username, password);

        var headers = ApplyHeaderConfigurators(builder);

        headers.Authorization.ShouldNotBeNull();
        headers.Authorization!.Scheme.ShouldBe("Basic");
        headers.Authorization.Parameter.ShouldNotBeNull();

        var encoded = headers.Authorization.Parameter!;
        var decodedBytes = Convert.FromBase64String(encoded);
        var decoded = Encoding.UTF8.GetString(decodedBytes);

        decoded.ShouldBe("user:pass");
    }

    [Fact]
    public void WithBasicAuthentication_ThrowsArgumentNullException_WhenUsernameIsNull()
    {
        var builder = CreateBuilder();

        var exception = Should.Throw<ArgumentNullException>(() =>
            builder.WithBasicAuthentication(null!, "password"));

        exception.ParamName.ShouldBe("username");
    }

    [Fact]
    public void WithBasicAuthentication_ThrowsArgumentNullException_WhenPasswordIsNull()
    {
        var builder = CreateBuilder();

        var exception = Should.Throw<ArgumentNullException>(() =>
            builder.WithBasicAuthentication("username", null!));

        exception.ParamName.ShouldBe("password");
    }

    [Fact]
    public void WithOAuthBearerToken_UsesBearerScheme_WhenTokenProvided()
    {
        var builder = CreateBuilder();
        var token = "access-token";

        builder.WithOAuthBearerToken(token);

        var headers = ApplyHeaderConfigurators(builder);

        headers.Authorization.ShouldNotBeNull();
        headers.Authorization!.Scheme.ShouldBe("Bearer");
        headers.Authorization.Parameter.ShouldBe(token);
    }

    [Fact]
    public void WithOAuthBearerToken_ThrowsArgumentNullException_WhenTokenIsNull()
    {
        var builder = CreateBuilder();

        var exception = Should.Throw<ArgumentNullException>(() =>
            builder.WithOAuthBearerToken(null!));

        exception.ParamName.ShouldBe("token");
    }

    [Fact]
    public void AuthenticationMethods_LastConfiguredAuthenticationHeader_WinsWhenMultipleAreRegistered()
    {
        var builder = CreateBuilder();

        builder
            .WithAuthentication("Custom", "first-token")
            .WithOAuthBearerToken("second-token");

        var headers = ApplyHeaderConfigurators(builder);

        headers.Authorization.ShouldNotBeNull();
        headers.Authorization!.Scheme.ShouldBe("Bearer");
        headers.Authorization.Parameter.ShouldBe("second-token");
    }
}
