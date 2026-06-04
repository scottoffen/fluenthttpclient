namespace FluentHttpClient.Tests;

public class BuilderFactoryTests
{
    [Fact]
    public void Create_FromClient_ReturnsBuilderInitializedWithClient()
    {
        using var client = new HttpClient();

        var builder = BuilderFactory<ValidBuilder>.Create(client);

        builder.ShouldBeOfType<ValidBuilder>();
        builder.CtorClient.ShouldBeSameAs(client);
    }

    [Fact]
    public void Create_FromRoute_ReturnsBuilderInitializedWithClientAndRoute()
    {
        using var client = new HttpClient();

        var builder = BuilderFactory<ValidBuilder>.Create(client, "api/widgets");

        builder.ShouldBeOfType<ValidBuilder>();
        builder.CtorClient.ShouldBeSameAs(client);
        builder.CtorRoute.ShouldBe("api/widgets");
    }

    [Fact]
    public void Create_FromUri_ReturnsBuilderInitializedWithClientAndUri()
    {
        using var client = new HttpClient();
        var uri = new Uri("https://example.com/api/widgets");

        var builder = BuilderFactory<ValidBuilder>.Create(client, uri);

        builder.ShouldBeOfType<ValidBuilder>();
        builder.CtorClient.ShouldBeSameAs(client);
        builder.CtorUri.ShouldBe(uri);
    }

    [Fact]
    public void StaticInit_MissingConstructor_ThrowsWithDescriptiveMessage()
    {
        using var client = new HttpClient();

        var ex = Should.Throw<TypeInitializationException>(
            () => BuilderFactory<MissingCtorBuilder>.Create(client));

        var inner = ex.InnerException.ShouldBeOfType<InvalidOperationException>();
        inner.Message.ShouldContain(nameof(MissingCtorBuilder));
        inner.Message.ShouldContain("HttpClient");
    }

    [Fact]
    public void StaticInit_PrivateConstructor_ThrowsWithDescriptiveMessage()
    {
        using var client = new HttpClient();

        var ex = Should.Throw<TypeInitializationException>(
            () => BuilderFactory<PrivateCtorBuilder>.Create(client));

        var inner = ex.InnerException.ShouldBeOfType<InvalidOperationException>();
        inner.Message.ShouldContain(nameof(PrivateCtorBuilder));
    }
}

internal class ValidBuilder : HttpRequestBuilder
{
    public HttpClient CtorClient { get; }
    public string? CtorRoute { get; }
    public Uri? CtorUri { get; }

    public ValidBuilder(HttpClient client) : base(client)
        => CtorClient = client;

    public ValidBuilder(HttpClient client, string route) : base(client, route)
        => (CtorClient, CtorRoute) = (client, route);

    public ValidBuilder(HttpClient client, Uri uri) : base(client, uri)
        => (CtorClient, CtorUri) = (client, uri);
}

internal class MissingCtorBuilder : HttpRequestBuilder
{
    // No public constructors matching any of the three required signatures.
    // The static constructor of BuilderFactory<MissingCtorBuilder> will fail
    // on the first ResolveCtor call (HttpClient).
    private MissingCtorBuilder(HttpClient client) : base(client) { }
}

internal class PrivateCtorBuilder : HttpRequestBuilder
{
    // All three signatures present but private, so GetConstructor with
    // BindingFlags.Public will not find them.
    private PrivateCtorBuilder(HttpClient client) : base(client) { }
    private PrivateCtorBuilder(HttpClient client, string route) : base(client, route) { }
    private PrivateCtorBuilder(HttpClient client, Uri uri) : base(client, uri) { }
}