using System.Net;
using System.Net.Http.Headers;

namespace FluentHttpClient.Tests;

public class HttpRequestBuilderTests
{
    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenClientIsNull()
    {
        Should.Throw<ArgumentNullException>(() => new HttpRequestBuilder(null!));
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenBaseAddressHasQuery()
    {
        var client = new HttpClient { BaseAddress = new Uri("https://api.example.com/v1?x=1") };

        var ex = Should.Throw<ArgumentException>(() => new HttpRequestBuilder(client));
        ex.ParamName.ShouldBe("client");
        ex.Message.ShouldContain("BaseAddress must not contain a query string or fragment", Case.Insensitive);
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenBaseAddressHasFragment()
    {
        var client = new HttpClient { BaseAddress = new Uri("https://api.example.com/v1#fragment") };

        var ex = Should.Throw<ArgumentException>(() => new HttpRequestBuilder(client));
        ex.ParamName.ShouldBe("client");
        ex.Message.ShouldContain("BaseAddress must not contain a query string or fragment", Case.Insensitive);
    }

    [Theory]
    [InlineData("route?x=1")]
    [InlineData("route#frag")]
    public void Route_ThrowsArgumentException_WhenValueContainsQueryOrFragment(string route)
    {
        var client = new HttpClient();
        var builder = new HttpRequestBuilder(client);

        var ex = Should.Throw<ArgumentException>(() => builder.Route = route);
        ex.ParamName.ShouldBe("Route");
        ex.Message.ShouldContain("Route must not contain a query string or fragment", Case.Insensitive);
    }

    [Fact]
    public void BuildRequestUri_ThrowsArgumentException_WhenBaseAddressAndRouteMissing()
    {
        var client = new HttpClient();
        var builder = new HttpRequestBuilder(client);

        var ex = Should.Throw<ArgumentException>(() => builder.BuildRequestUri());
        ex.ParamName.ShouldBe("Route");
        ex.Message.ShouldContain("Invalid route", Case.Insensitive);
    }

    [Fact]
    public void BuildRequestUri_ReturnsBaseAddress_WhenRouteMissingAndNoQueryParameters()
    {
        var client = new HttpClient { BaseAddress = new Uri("https://api.example.com/v1/") };
        var builder = new HttpRequestBuilder(client);

        var uri = builder.BuildRequestUri();

        uri.ShouldNotBeNull();
        uri.ToString().ShouldBe("https://api.example.com/v1");
    }

    [Fact]
    public void BuildRequestUri_AppendsRouteToBaseAddress_WhenBothProvided()
    {
        var client = new HttpClient { BaseAddress = new Uri("https://api.example.com/v1/") };
        var builder = new HttpRequestBuilder(client, "users/");

        var uri = builder.BuildRequestUri();

        uri.ShouldNotBeNull();
        uri.ToString().ShouldBe("https://api.example.com/v1/users");
    }

    [Fact]
    public void BuildRequestUri_UsesRouteOnly_WhenBaseAddressMissing()
    {
        var client = new HttpClient();
        var builder = new HttpRequestBuilder(client)
        {
            Route = "api/users"
        };

        var uri = builder.BuildRequestUri();

        uri.ShouldNotBeNull();
        uri.ToString().ShouldBe("api/users");
        uri.IsAbsoluteUri.ShouldBeFalse();
    }

    [Fact]
    public void BuildRequestUri_AppendsQueryString_WhenParametersPresent()
    {
        var client = new HttpClient { BaseAddress = new Uri("https://api.example.com/") };
        var builder = new HttpRequestBuilder(client, "search");

        builder.QueryParameters.Add("q", "test");
        builder.QueryParameters.Add("page", "1");

        var uri = builder.BuildRequestUri();

        uri.ShouldNotBeNull();
        uri.ToString().ShouldBe("https://api.example.com/search?q=test&page=1");
    }

    [Fact]
    public async Task BuildRequest_SetsContentVersionAndPolicy_WhenCalled()
    {
        var client = new HttpClient { BaseAddress = new Uri("https://api.example.com/") };
        var builder = new HttpRequestBuilder(client, "items")
        {
            Content = new StringContent("payload"),
            Version = HttpVersion.Version20,
            VersionPolicy = HttpVersionPolicy.RequestVersionOrHigher
        };

        var request = await builder.BuildRequest(HttpMethod.Post, CancellationToken.None);

        request.Method.ShouldBe(HttpMethod.Post);
        request.RequestUri!.ToString().ShouldBe("https://api.example.com/items");
        request.Content.ShouldBe(builder.Content);
        request.Version.ShouldBe(HttpVersion.Version20);
        request.VersionPolicy.ShouldBe(HttpVersionPolicy.RequestVersionOrHigher);
    }

    [Fact]
    public async Task BuildRequest_AppliesHeaderConfigurators_WhenPresent()
    {
        var client = new HttpClient { BaseAddress = new Uri("https://api.example.com/") };
        var builder = new HttpRequestBuilder(client, "items");

        builder.HeaderConfigurators.Add(headers =>
        {
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            headers.Add("X-Test", "value");
        });

        var request = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);

        request.Headers.Accept.Single().MediaType.ShouldBe("application/json");
        request.Headers.GetValues("X-Test").Single().ShouldBe("value");
    }

    [Fact]
    public async Task BuildRequest_AppliesCookiesToCookieHeader_WhenCookiesPresent()
    {
        var client = new HttpClient { BaseAddress = new Uri("https://api.example.com/") };
        var builder = new HttpRequestBuilder(client, "items");

        builder.Cookies["session"] = "abc123";
        builder.Cookies["mode"] = "test";

        var request = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);

        request.Headers.TryGetValues("Cookie", out var cookieHeaders).ShouldBeTrue();
        var cookieHeader = cookieHeaders.Single();

        cookieHeader.ShouldContain("session=abc123");
        cookieHeader.ShouldContain("mode=test");
        cookieHeader.ShouldBe("session=abc123; mode=test");
    }

    [Fact]
    public async Task BuildRequest_AppliesOptionConfigurators_WhenPresent()
    {
        var client = new HttpClient { BaseAddress = new Uri("https://api.example.com/") };
        var builder = new HttpRequestBuilder(client, "items");

        var key = new HttpRequestOptionsKey<string>("test-option");

        builder.OptionConfigurators.Add(options =>
        {
            options.Set(key, "value");
        });

        var request = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);

        request.Options.TryGetValue(key, out string? value).ShouldBeTrue();
        value.ShouldBe("value");
    }

    [Fact]
    public async Task BuildRequest_DisablesExpectContinue_WhenContentIsMultipart()
    {
        var client = new HttpClient { BaseAddress = new Uri("https://api.example.com/") };
        var builder = new HttpRequestBuilder(client, "upload")
        {
            Content = new MultipartFormDataContent()
        };

        var request = await builder.BuildRequest(HttpMethod.Post, CancellationToken.None);

        request.Headers.ExpectContinue!.Value.ShouldBeFalse();
    }

    [Fact]
    public async Task BuildRequest_DoesNotModifyExpectContinue_WhenContentIsNotMultipart()
    {
        var client = new HttpClient { BaseAddress = new Uri("https://api.example.com/") };
        var builder = new HttpRequestBuilder(client, "items")
        {
            Content = new StringContent("payload")
        };

        var request = await builder.BuildRequest(HttpMethod.Post, CancellationToken.None);

        request.Headers.ExpectContinue.ShouldBeNull();
    }

    [Fact]
    public async Task BuildRequest_BuffersContent_WhenBufferContentBeforeSendingIsTrue()
    {
        var client = new HttpClient { BaseAddress = new Uri("https://api.example.com/") };
        var builder = new HttpRequestBuilder(client, "items")
        {
            Content = new TrackingContent(),
            BufferContentBeforeSending = true
        };

        var trackingContent = (TrackingContent)builder.Content;

        var request = await builder.BuildRequest(HttpMethod.Post, CancellationToken.None);

        request.ShouldNotBeNull();
        trackingContent.SerializeCalled.ShouldBeTrue();
    }

    [Fact]
    public async Task BuildRequest_DoesNotBufferContent_WhenBufferContentBeforeSendingIsFalse()
    {
        var client = new HttpClient { BaseAddress = new Uri("https://api.example.com/") };
        var builder = new HttpRequestBuilder(client, "items")
        {
            Content = new TrackingContent(),
            BufferContentBeforeSending = false
        };

        var trackingContent = (TrackingContent)builder.Content;

        var request = await builder.BuildRequest(HttpMethod.Post, CancellationToken.None);

        request.ShouldNotBeNull();
        trackingContent.SerializeCalled.ShouldBeFalse();
    }

    [Fact]
    public async Task BuildRequest_ThrowsArgumentNullException_WhenMethodIsNull()
    {
        var client = new HttpClient { BaseAddress = new Uri("https://api.example.com/") };
        var builder = new HttpRequestBuilder(client, "items");

        Should.Throw<ArgumentNullException>(() =>
            builder.BuildRequest(null!, CancellationToken.None).GetAwaiter().GetResult());
    }

    private sealed class TrackingContent : HttpContent
    {
        public bool SerializeCalled { get; private set; }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        {
            SerializeCalled = true;
            return Task.CompletedTask;
        }

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return true;
        }
    }
}
