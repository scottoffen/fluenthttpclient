using System.Net;

namespace FluentHttpClient.Tests;

public class FluentSendExtensionsTests
{
    private static HttpRequestBuilder CreateBuilder(TestHttpMessageHandler handler)
    {
        var client = new HttpClient(handler);
        return new HttpRequestBuilder(client, "https://example.com/");
    }

    private sealed class TestHttpMessageHandler : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }
        public CancellationToken LastCancellationToken { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            LastRequest = request;
            LastCancellationToken = cancellationToken;

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                RequestMessage = request
            };

            return Task.FromResult(response);
        }
    }

    public class DeleteAsyncTests
    {
        [Fact]
        public async Task DeleteAsync_UsesDeleteMethod_WhenCalledWithoutParameters()
        {
            var handler = new TestHttpMessageHandler();
            var builder = CreateBuilder(handler);

            var response = await builder.DeleteAsync();

            response.ShouldNotBeNull();
            handler.LastRequest.ShouldNotBeNull();
            handler.LastRequest!.Method.ShouldBe(HttpMethod.Delete);
        }

        [Fact]
        public async Task DeleteAsync_UsesDeleteMethod_WhenCancellationTokenProvided()
        {
            var handler = new TestHttpMessageHandler();
            var builder = CreateBuilder(handler);
            using var cts = new CancellationTokenSource();

            var response = await builder.DeleteAsync(cts.Token);

            response.ShouldNotBeNull();
            handler.LastRequest.ShouldNotBeNull();
            handler.LastRequest!.Method.ShouldBe(HttpMethod.Delete);
        }

        [Fact]
        public async Task DeleteAsync_UsesDeleteMethod_WhenCompletionOptionProvided()
        {
            var handler = new TestHttpMessageHandler();
            var builder = CreateBuilder(handler);

            var response = await builder.DeleteAsync(HttpCompletionOption.ResponseHeadersRead);

            response.ShouldNotBeNull();
            handler.LastRequest.ShouldNotBeNull();
            handler.LastRequest!.Method.ShouldBe(HttpMethod.Delete);
        }

        [Fact]
        public async Task DeleteAsync_UsesDeleteMethod_WhenCompletionOptionAndCancellationTokenProvided()
        {
            var handler = new TestHttpMessageHandler();
            var builder = CreateBuilder(handler);
            using var cts = new CancellationTokenSource();

            var response = await builder.DeleteAsync(HttpCompletionOption.ResponseContentRead, cts.Token);

            response.ShouldNotBeNull();
            handler.LastRequest.ShouldNotBeNull();
            handler.LastRequest!.Method.ShouldBe(HttpMethod.Delete);
        }
    }

    public class GetAsyncTests
    {
        [Fact]
        public async Task GetAsync_UsesGetMethod_WhenCalledWithoutParameters()
        {
            var handler = new TestHttpMessageHandler();
            var builder = CreateBuilder(handler);

            var response = await builder.GetAsync();

            response.ShouldNotBeNull();
            handler.LastRequest.ShouldNotBeNull();
            handler.LastRequest!.Method.ShouldBe(HttpMethod.Get);
        }

        [Fact]
        public async Task GetAsync_UsesGetMethod_WhenCancellationTokenProvided()
        {
            var handler = new TestHttpMessageHandler();
            var builder = CreateBuilder(handler);
            using var cts = new CancellationTokenSource();

            var response = await builder.GetAsync(cts.Token);

            response.ShouldNotBeNull();
            handler.LastRequest.ShouldNotBeNull();
            handler.LastRequest!.Method.ShouldBe(HttpMethod.Get);
        }

        [Fact]
        public async Task GetAsync_UsesGetMethod_WhenCompletionOptionProvided()
        {
            var handler = new TestHttpMessageHandler();
            var builder = CreateBuilder(handler);

            var response = await builder.GetAsync(HttpCompletionOption.ResponseHeadersRead);

            response.ShouldNotBeNull();
            handler.LastRequest.ShouldNotBeNull();
            handler.LastRequest!.Method.ShouldBe(HttpMethod.Get);
        }

        [Fact]
        public async Task GetAsync_UsesGetMethod_WhenCompletionOptionAndCancellationTokenProvided()
        {
            var handler = new TestHttpMessageHandler();
            var builder = CreateBuilder(handler);
            using var cts = new CancellationTokenSource();

            var response = await builder.GetAsync(HttpCompletionOption.ResponseContentRead, cts.Token);

            response.ShouldNotBeNull();
            handler.LastRequest.ShouldNotBeNull();
            handler.LastRequest!.Method.ShouldBe(HttpMethod.Get);
        }
    }

    public class PatchAsyncTests
    {
        [Fact]
        public async Task PatchAsync_UsesPatchMethod_WhenCalledWithoutParameters()
        {
            var handler = new TestHttpMessageHandler();
            var builder = CreateBuilder(handler);

            var response = await builder.PatchAsync();

            response.ShouldNotBeNull();
            handler.LastRequest.ShouldNotBeNull();
            handler.LastRequest!.Method.ShouldBe(HttpMethod.Patch);
        }

        [Fact]
        public async Task PatchAsync_UsesPatchMethod_WhenCancellationTokenProvided()
        {
            var handler = new TestHttpMessageHandler();
            var builder = CreateBuilder(handler);
            using var cts = new CancellationTokenSource();

            var response = await builder.PatchAsync(cts.Token);

            response.ShouldNotBeNull();
            handler.LastRequest.ShouldNotBeNull();
            handler.LastRequest!.Method.ShouldBe(HttpMethod.Patch);
        }

        [Fact]
        public async Task PatchAsync_UsesPatchMethod_WhenCompletionOptionProvided()
        {
            var handler = new TestHttpMessageHandler();
            var builder = CreateBuilder(handler);

            var response = await builder.PatchAsync(HttpCompletionOption.ResponseHeadersRead);

            response.ShouldNotBeNull();
            handler.LastRequest.ShouldNotBeNull();
            handler.LastRequest!.Method.ShouldBe(HttpMethod.Patch);
        }

        [Fact]
        public async Task PatchAsync_UsesPatchMethod_WhenCompletionOptionAndCancellationTokenProvided()
        {
            var handler = new TestHttpMessageHandler();
            var builder = CreateBuilder(handler);
            using var cts = new CancellationTokenSource();

            var response = await builder.PatchAsync(HttpCompletionOption.ResponseContentRead, cts.Token);

            response.ShouldNotBeNull();
            handler.LastRequest.ShouldNotBeNull();
            handler.LastRequest!.Method.ShouldBe(HttpMethod.Patch);
        }
    }

    public class PostAsyncTests
    {
        [Fact]
        public async Task PostAsync_UsesPostMethod_WhenCalledWithoutParameters()
        {
            var handler = new TestHttpMessageHandler();
            var builder = CreateBuilder(handler);

            var response = await builder.PostAsync();

            response.ShouldNotBeNull();
            handler.LastRequest.ShouldNotBeNull();
            handler.LastRequest!.Method.ShouldBe(HttpMethod.Post);
        }

        [Fact]
        public async Task PostAsync_UsesPostMethod_WhenCancellationTokenProvided()
        {
            var handler = new TestHttpMessageHandler();
            var builder = CreateBuilder(handler);
            using var cts = new CancellationTokenSource();

            var response = await builder.PostAsync(cts.Token);

            response.ShouldNotBeNull();
            handler.LastRequest.ShouldNotBeNull();
            handler.LastRequest!.Method.ShouldBe(HttpMethod.Post);
        }

        [Fact]
        public async Task PostAsync_UsesPostMethod_WhenCompletionOptionProvided()
        {
            var handler = new TestHttpMessageHandler();
            var builder = CreateBuilder(handler);

            var response = await builder.PostAsync(HttpCompletionOption.ResponseHeadersRead);

            response.ShouldNotBeNull();
            handler.LastRequest.ShouldNotBeNull();
            handler.LastRequest!.Method.ShouldBe(HttpMethod.Post);
        }

        [Fact]
        public async Task PostAsync_UsesPostMethod_WhenCompletionOptionAndCancellationTokenProvided()
        {
            var handler = new TestHttpMessageHandler();
            var builder = CreateBuilder(handler);
            using var cts = new CancellationTokenSource();

            var response = await builder.PostAsync(HttpCompletionOption.ResponseContentRead, cts.Token);

            response.ShouldNotBeNull();
            handler.LastRequest.ShouldNotBeNull();
            handler.LastRequest!.Method.ShouldBe(HttpMethod.Post);
        }
    }

    public class PutAsyncTests
    {
        [Fact]
        public async Task PutAsync_UsesPutMethod_WhenCalledWithoutParameters()
        {
            var handler = new TestHttpMessageHandler();
            var builder = CreateBuilder(handler);

            var response = await builder.PutAsync();

            response.ShouldNotBeNull();
            handler.LastRequest.ShouldNotBeNull();
            handler.LastRequest!.Method.ShouldBe(HttpMethod.Put);
        }

        [Fact]
        public async Task PutAsync_UsesPutMethod_WhenCancellationTokenProvided()
        {
            var handler = new TestHttpMessageHandler();
            var builder = CreateBuilder(handler);
            using var cts = new CancellationTokenSource();

            var response = await builder.PutAsync(cts.Token);

            response.ShouldNotBeNull();
            handler.LastRequest.ShouldNotBeNull();
            handler.LastRequest!.Method.ShouldBe(HttpMethod.Put);
        }

        [Fact]
        public async Task PutAsync_UsesPutMethod_WhenCompletionOptionProvided()
        {
            var handler = new TestHttpMessageHandler();
            var builder = CreateBuilder(handler);

            var response = await builder.PutAsync(HttpCompletionOption.ResponseHeadersRead);

            response.ShouldNotBeNull();
            handler.LastRequest.ShouldNotBeNull();
            handler.LastRequest!.Method.ShouldBe(HttpMethod.Put);
        }

        [Fact]
        public async Task PutAsync_UsesPutMethod_WhenCompletionOptionAndCancellationTokenProvided()
        {
            var handler = new TestHttpMessageHandler();
            var builder = CreateBuilder(handler);
            using var cts = new CancellationTokenSource();

            var response = await builder.PutAsync(HttpCompletionOption.ResponseContentRead, cts.Token);

            response.ShouldNotBeNull();
            handler.LastRequest.ShouldNotBeNull();
            handler.LastRequest!.Method.ShouldBe(HttpMethod.Put);
        }
    }
}
