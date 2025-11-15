using System.Net;

namespace FluentHttpClient.Tests;

public class HttpRequestBuilderSendAsyncTests
{
    [Fact]
    public async Task SendAsync_UsesDefaultCompletionOptionAndCancellationToken_WhenNotSpecified()
    {
        var handler = new TestHttpMessageHandler();
        using var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.example.com/") };

        var builder = new HttpRequestBuilder(client, "items");

        var response = await builder.SendAsync(HttpMethod.Get);

        response.ShouldNotBeNull();
        handler.CallCount.ShouldBe(1);
        handler.LastRequest.ShouldNotBeNull();
        handler.LastRequest!.Method.ShouldBe(HttpMethod.Get);
    }

    [Fact]
    public async Task SendAsync_UsesProvidedCancellationToken_WhenOverloadWithTokenIsCalled()
    {
        var handler = new TestHttpMessageHandler();
        using var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.example.com/") };

        var builder = new HttpRequestBuilder(client, "items");
        using var cts = new CancellationTokenSource();

        var response = await builder.SendAsync(HttpMethod.Post, cts.Token);

        response.ShouldNotBeNull();
        handler.CallCount.ShouldBe(1);
        handler.LastRequest.ShouldNotBeNull();
        handler.LastRequest!.Method.ShouldBe(HttpMethod.Post);
    }

    [Fact]
    public async Task SendAsync_UsesProvidedCompletionOption_WhenCompletionOptionOverloadCalled()
    {
        var handler = new TestHttpMessageHandler();
        using var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.example.com/") };

        var builder = new HttpRequestBuilder(client, "items");

        var response = await builder.SendAsync(HttpMethod.Get, HttpCompletionOption.ResponseHeadersRead);

        response.ShouldNotBeNull();
        handler.CallCount.ShouldBe(1);
        handler.LastRequest.ShouldNotBeNull();
        handler.LastRequest!.Method.ShouldBe(HttpMethod.Get);
    }

    [Fact]
    public async Task SendAsync_UsesProvidedCompletionOptionAndCancellationToken_WhenAllParametersSpecified()
    {
        var handler = new TestHttpMessageHandler();
        using var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.example.com/") };

        var builder = new HttpRequestBuilder(client, "items");
        using var cts = new CancellationTokenSource();

        var response = await builder.SendAsync(
            HttpMethod.Delete,
            HttpCompletionOption.ResponseHeadersRead,
            cts.Token);

        response.ShouldNotBeNull();
        handler.CallCount.ShouldBe(1);
        handler.LastRequest.ShouldNotBeNull();
        handler.LastRequest!.Method.ShouldBe(HttpMethod.Delete);
    }

    [Fact]
    public async Task SendAsync_PropagatesExceptions_FromHttpClient()
    {
        var handler = new TestHttpMessageHandler
        {
            ThrowOnSend = true
        };

        using var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.example.com/") };

        var builder = new HttpRequestBuilder(client, "items");

        var exception = await Should.ThrowAsync<HttpRequestException>(
            async () => await builder.SendAsync(HttpMethod.Get));

        exception.Message.ShouldContain("Test failure");
        handler.CallCount.ShouldBe(1);
    }

    private sealed class TestHttpMessageHandler : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }

        public CancellationToken LastCancellationToken { get; private set; }

        public int CallCount { get; private set; }

        public bool ThrowOnSend { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            CallCount++;
            LastRequest = request;
            LastCancellationToken = cancellationToken;

            if (ThrowOnSend)
            {
                throw new HttpRequestException("Test failure");
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
