using System.Net;

namespace FluentHttpClient.Tests;

public class HttpRequestBuilderTests
{
    private static HttpClient CreateClient(string? baseAddress = null)
    {
        var client = new HttpClient();
        if (baseAddress is not null)
        {
            client.BaseAddress = new Uri(baseAddress);
        }

        return client;
    }

    public class ConstructorTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenClientIsNull()
        {
            Should.Throw<ArgumentNullException>(() => new HttpRequestBuilder(null!));
        }

        [Theory]
        [InlineData("https://api.example.com/v1/")]
        [InlineData("https://api.example.com/")]
        public void Ctor_AllowsClientWithBaseAddressWithoutQueryOrFragment(string baseAddress)
        {
            using var client = CreateClient(baseAddress);

            var builder = new HttpRequestBuilder(client);

            builder.Route.ShouldBeNull();
        }

        [Theory]
        [InlineData("https://api.example.com/v1/?x=1")]
        [InlineData("https://api.example.com/v1/#fragment")]
        [InlineData("https://api.example.com/v1/?x=1#fragment")]
        public void Ctor_ThrowsArgumentException_WhenBaseAddressHasQueryOrFragment(string baseAddress)
        {
            using var client = CreateClient(baseAddress);

            Should.Throw<ArgumentException>(() => new HttpRequestBuilder(client));
        }

        [Fact]
        public void CtorWithStringRoute_ThrowsArgumentNullException_WhenRouteIsNull()
        {
            using var client = CreateClient();

            Should.Throw<ArgumentNullException>(() => new HttpRequestBuilder(client, (string)null!));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void CtorWithStringRoute_ThrowsArgumentException_WhenRouteIsEmptyOrWhitespace(string route)
        {
            using var client = CreateClient();

            Should.Throw<ArgumentException>(() => new HttpRequestBuilder(client, route));
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("/foo/bar")]
        [InlineData(" foo ")]
        public void CtorWithStringRoute_SetsRoute_WhenValueIsValid(string route)
        {
            using var client = CreateClient();

            var builder = new HttpRequestBuilder(client, route);

            builder.Route.ShouldBe(route.Trim());
        }

        [Theory]
        [InlineData("foo?x=1")]
        [InlineData("/foo#frag")]
        [InlineData("https://api.example.com/foo?x=1")]
        public void CtorWithStringRoute_ThrowsArgumentException_WhenRouteHasQueryOrFragment(string route)
        {
            using var client = CreateClient();

            Should.Throw<ArgumentException>(() => new HttpRequestBuilder(client, route));
        }

        [Fact]
        public void CtorWithUriRoute_ThrowsArgumentNullException_WhenRouteIsNull()
        {
            using var client = CreateClient();

            Should.Throw<ArgumentNullException>(() => new HttpRequestBuilder(client, (Uri)null!));
        }

        [Theory]
        [InlineData("https://api.example.com/foo?x=1")]
        [InlineData("https://api.example.com/foo#frag")]
        public void CtorWithUriRoute_ThrowsArgumentException_WhenRouteHasQueryOrFragment(string uri)
        {
            using var client = CreateClient();
            var route = new Uri(uri);

            Should.Throw<ArgumentException>(() => new HttpRequestBuilder(client, route));
        }

        [Theory]
        [InlineData("https://api.example.com/foo")]
        [InlineData("/foo")]
        [InlineData("foo/bar")]
        public void CtorWithUriRoute_SetsRoute_WhenRouteIsValid(string uriText)
        {
            using var client = CreateClient();
            var route = new Uri(uriText, UriKind.RelativeOrAbsolute);

            var builder = new HttpRequestBuilder(client, route);

            builder.Route.ShouldBe(route.OriginalString);
        }
    }

    public class RouteAndPropertiesTests
    {
        [Fact]
        public void Route_IsNull_WhenConstructedWithClientOnly()
        {
            using var client = CreateClient();

            var builder = new HttpRequestBuilder(client);

            builder.Route.ShouldBeNull();
        }

        [Fact]
        public void VersionAndVersionPolicy_HaveExpectedDefaults()
        {
            using var client = CreateClient();

            var builder = new HttpRequestBuilder(client);

            builder.Version.ShouldBe(HttpVersion.Version11);
            builder.VersionPolicy.ShouldBe(HttpVersionPolicy.RequestVersionOrLower);
        }

        [Fact]
        public void Cookies_HeaderConfigurators_And_OptionConfigurators_AreInitialized()
        {
            using var client = CreateClient();

            var builder = new HttpRequestBuilder(client);

            builder.Cookies.ShouldNotBeNull();
            builder.HeaderConfigurators.ShouldNotBeNull();
            builder.OptionConfigurators.ShouldNotBeNull();
        }
    }

    public class DeferredConfiguratorTests
    {
        [Fact]
        public async Task BuildRequest_ExecutesDeferredConfigurators_WhenBuildingRequest()
        {
            var client = CreateClient();
            var builder = new HttpRequestBuilder(client, "https://www.example.com");

            builder.DeferredConfigurators.Add(b =>
            {
                b.WithHeader("X-Deferred", "true");
                b.WithQueryParameter("deferred", "1");
            });

            var request = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);

            request.Headers.Contains("X-Deferred").ShouldBeTrue();
            request.Headers.GetValues("X-Deferred").ShouldContain("true");

            request.RequestUri.ShouldNotBeNull();
            request.RequestUri!.Query.ShouldContain("deferred=1");
        }

        [Fact]
        public async Task BuildRequest_ExecutesAllDeferredConfigurators_WhenMultipleAreRegistered()
        {
            var client = CreateClient();
            var builder = new HttpRequestBuilder(client, "https://www.example.com");

            builder.DeferredConfigurators.Add(b => b.WithHeader("X-First", "1"));
            builder.DeferredConfigurators.Add(b => b.WithHeader("X-Second", "2"));
            builder.DeferredConfigurators.Add(b => b.WithQueryParameter("first", "one"));

            var request = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);

            request.Headers.Contains("X-First").ShouldBeTrue();
            request.Headers.GetValues("X-First").ShouldContain("1");

            request.Headers.Contains("X-Second").ShouldBeTrue();
            request.Headers.GetValues("X-Second").ShouldContain("2");

            request.RequestUri.ShouldNotBeNull();
            request.RequestUri!.Query.ShouldContain("first=one");
        }

        [Fact]
        public async Task BuildRequest_ExecutesDeferredConfiguratorsOnEveryCall_WhenBuildingMultipleRequests()
        {
            var client = CreateClient();
            var builder = new HttpRequestBuilder(client, "https://www.example.com");
            var invocationCount = 0;

            builder.DeferredConfigurators.Add(b =>
            {
                invocationCount++;
                b.WithHeader("X-Invocation", invocationCount.ToString());
            });

            var firstRequest = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);
            var secondRequest = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);

            invocationCount.ShouldBe(2);

            firstRequest.Headers.Contains("X-Invocation").ShouldBeTrue();
            firstRequest.Headers.GetValues("X-Invocation").ShouldContain("1");

            secondRequest.Headers.Contains("X-Invocation").ShouldBeTrue();
            secondRequest.Headers.GetValues("X-Invocation").ShouldContain("2");
        }

        [Fact]
        public async Task BuildRequest_DoesNothingSpecial_WhenNoDeferredConfiguratorsAreRegistered()
        {
            var client = CreateClient();
            var builder = new HttpRequestBuilder(client, "https://www.example.com");

            var request = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);

            request.ShouldNotBeNull();
            request.RequestUri.ShouldNotBeNull();
            request.Headers.ShouldBeEmpty();
        }
    }

    public class BuildRequestUriTests
    {
        [Fact]
        public void BuildRequestUri_ThrowsArgumentException_WhenBaseAddressAndRouteAreMissing()
        {
            using var client = CreateClient();
            var builder = new HttpRequestBuilder(client);

            Should.Throw<ArgumentException>(() => builder.BuildRequestUri());
        }

        [Fact]
        public void BuildRequestUri_ReturnsEmptyRelativeUri_WhenBaseAddressExistsAndNoRouteOrQuery()
        {
            using var client = CreateClient("https://api.example.com/v1/");
            var builder = new HttpRequestBuilder(client);

            var uri = builder.BuildRequestUri();

            uri.IsAbsoluteUri.ShouldBeFalse();
            uri.ToString().ShouldBe(string.Empty);
        }

        [Fact]
        public void BuildRequestUri_ReturnsRelativeQueryOnly_WhenBaseAddressExistsAndNoRoute()
        {
            using var client = CreateClient("https://api.example.com/v1/");
            var builder = new HttpRequestBuilder(client);
            builder.QueryParameters.Add("foo", "bar");

            var uri = builder.BuildRequestUri();

            uri.IsAbsoluteUri.ShouldBeFalse();
            uri.ToString().ShouldBe("?foo=bar");
        }

        [Fact]
        public void BuildRequestUri_ReturnsAbsoluteUriWithQuery_WhenRouteIsAbsolute()
        {
            using var client = CreateClient("https://ignored.example.com/base/");
            var route = new Uri("https://api.example.com/resource", UriKind.Absolute);
            var builder = new HttpRequestBuilder(client, route);
            builder.QueryParameters.Add("foo", "bar");

            var uri = builder.BuildRequestUri();

            uri.IsAbsoluteUri.ShouldBeTrue();
            uri.ToString().ShouldBe("https://api.example.com/resource?foo=bar");
        }

        [Fact]
        public void BuildRequestUri_ReturnsRelativeUri_WhenRouteIsRelativeAndNoBaseAddress()
        {
            using var client = CreateClient();
            var builder = new HttpRequestBuilder(client, "foo/bar");

            var uri = builder.BuildRequestUri();

            uri.IsAbsoluteUri.ShouldBeFalse();
            uri.ToString().ShouldBe("foo/bar");
        }

        [Fact]
        public void BuildRequestUri_ReturnsRootRelativeUri_WhenRouteIsRootRelativeAndNoBaseAddress()
        {
            using var client = CreateClient();
            var builder = new HttpRequestBuilder(client, "/foo/bar");

            var uri = builder.BuildRequestUri();

            uri.IsAbsoluteUri.ShouldBeFalse();
            uri.ToString().ShouldBe("/foo/bar");
        }

        [Fact]
        public void BuildRequestUri_PreservesRouteWhitespaceSemantics_ByTrimming()
        {
            using var client = CreateClient();
            var builder = new HttpRequestBuilder(client, "  foo/bar  ");

            var uri = builder.BuildRequestUri();

            uri.IsAbsoluteUri.ShouldBeFalse();
            uri.ToString().ShouldBe("foo/bar");
        }
    }

    public class BuildRequestTests
    {
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

        [Fact]
        public async Task BuildRequest_ThrowsArgumentNullException_WhenMethodIsNull()
        {
            using var client = CreateClient("https://api.example.com/");
            var builder = new HttpRequestBuilder(client);

            await Should.ThrowAsync<ArgumentNullException>(
                () => builder.BuildRequest(null!, CancellationToken.None));
        }

        [Fact]
        public async Task BuildRequest_SetsHttpRequestMessageProperties_FromBuilder()
        {
            using var client = CreateClient("https://api.example.com/");
            var builder = new HttpRequestBuilder(client, "foo");
            builder.Version = HttpVersion.Version20;
            builder.VersionPolicy = HttpVersionPolicy.RequestVersionExact;

            var content = new StringContent("hello");
            builder.Content = content;

            var request = await builder.BuildRequest(HttpMethod.Post, CancellationToken.None);

            request.Method.ShouldBe(HttpMethod.Post);
            request.RequestUri.ShouldNotBeNull();
            request.Version.ShouldBe(HttpVersion.Version20);
            request.VersionPolicy.ShouldBe(HttpVersionPolicy.RequestVersionExact);
            request.Content.ShouldBeSameAs(content);
        }

        [Fact]
        public async Task BuildRequest_LoadsContentIntoBuffer_WhenBufferRequestContentIsTrue()
        {
            using var client = CreateClient("https://api.example.com/");
            var builder = new HttpRequestBuilder(client, "foo");
            builder.BufferRequestContent = true;
            var trackingContent = new TrackingContent();
            builder.Content = trackingContent;

            _ = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);

            trackingContent.SerializeCalled.ShouldBeTrue();
        }
    }

    public class ApplyConfigurationTests
    {
        [Fact]
        public async Task BuildRequest_DisablesExpectContinue_WhenContentIsMultipart()
        {
            using var client = CreateClient("https://api.example.com/");
            var builder = new HttpRequestBuilder(client, "foo");
            builder.Content = new MultipartContent();

            var request = await builder.BuildRequest(HttpMethod.Post, CancellationToken.None);

            request.Headers.ExpectContinue!.Value.ShouldBeFalse();
        }

        [Fact]
        public async Task BuildRequest_InvokesHeaderConfigurators()
        {
            using var client = CreateClient("https://api.example.com/");
            var builder = new HttpRequestBuilder(client, "foo");

            builder.HeaderConfigurators.Add(headers =>
            {
                headers.Add("X-Test-Header", "abc");
            });

            var request = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);

            request.Headers.TryGetValues("X-Test-Header", out var values).ShouldBeTrue();
            values.ShouldContain("abc");
        }

        [Fact]
        public async Task BuildRequest_AddsCookieHeader_WhenCookiesArePresent()
        {
            using var client = CreateClient("https://api.example.com/");
            var builder = new HttpRequestBuilder(client, "foo");

            builder.Cookies["session"] = "123";
            builder.Cookies["user"] = "scott";

            var request = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);

            request.Headers.TryGetValues("Cookie", out var values).ShouldBeTrue();
            var cookieHeader = values.Single();
            cookieHeader.ShouldContain("session=123");
            cookieHeader.ShouldContain("user=scott");
            cookieHeader.ShouldContain("; ");
        }

        [Fact]
        public async Task BuildRequest_DoesNotAddCookieHeader_WhenNoCookies()
        {
            using var client = CreateClient("https://api.example.com/");
            var builder = new HttpRequestBuilder(client, "foo");

            var request = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);

            request.Headers.Contains("Cookie").ShouldBeFalse();
        }

        [Fact]
        public async Task BuildRequest_InvokesOptionConfigurators()
        {
            using var client = CreateClient("https://api.example.com/");
            var builder = new HttpRequestBuilder(client, "foo");

            var key = new HttpRequestOptionsKey<string>("test-key");
            builder.OptionConfigurators.Add(options =>
            {
                options.Set(key, "value");
            });

            var request = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);

            request.Options.TryGetValue(key, out var value).ShouldBeTrue();
            value.ShouldBe("value");
        }
    }

    public class CreateRouteUriTests
    {
        [Fact]
        public void CreateRouteUri_ThrowsArgumentNullException_WhenRouteIsNull()
        {
            Should.Throw<ArgumentNullException>(() => HttpRequestBuilder.CreateRouteUri(null!));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateRouteUri_ThrowsArgumentException_WhenRouteIsEmptyOrWhitespace(string route)
        {
            Should.Throw<ArgumentException>(() => HttpRequestBuilder.CreateRouteUri(route));
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("/foo/bar")]
        public void CreateRouteUri_ReturnsRelativeUri_WhenRouteIsRelative(string route)
        {
            var uri = HttpRequestBuilder.CreateRouteUri(route);

            uri.IsAbsoluteUri.ShouldBeFalse();
            uri.OriginalString.ShouldBe(route.Trim());
        }

        [Theory]
        [InlineData("https://api.example.com/foo")]
        [InlineData("http://example.org/")]
        public void CreateRouteUri_ReturnsAbsoluteUri_WhenRouteIsAbsolute(string route)
        {
            var uri = HttpRequestBuilder.CreateRouteUri(route);

            uri.IsAbsoluteUri.ShouldBeTrue();
            uri.OriginalString.ShouldBe(route.Trim());
        }

        [Theory]
        [InlineData("http://")]
        public void CreateRouteUri_ThrowsArgumentException_WhenRouteIsMalformed(string route)
        {
            Should.Throw<ArgumentException>(() => HttpRequestBuilder.CreateRouteUri(route));
        }
    }

    public class SendAsyncTests
    {
        // These tests do not use the CreateClient method, because they
        // are using a custom handler in order to assert behavior.

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

    public class SendAsyncStringTests
    {
        private sealed class TestHttpMessageHandler : HttpMessageHandler
        {
            public HttpRequestMessage? LastRequest { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                LastRequest = request;

                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    RequestMessage = request
                };

                return Task.FromResult(response);
            }
        }

        private static HttpRequestBuilder CreateBuilder(TestHttpMessageHandler handler)
        {
            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://example.com/")
            };

            // Constructors are internal, tests rely on InternalsVisibleTo
            return new HttpRequestBuilder(client);
        }

        public class MethodOnlyOverload
        {
            [Fact]
            public async Task SendAsync_UsesUppercaseHttpMethod_WhenOnlyMethodProvided()
            {
                var handler = new TestHttpMessageHandler();
                var builder = CreateBuilder(handler);

                var response = await builder.SendAsync("get");

                response.ShouldNotBeNull();
                handler.LastRequest.ShouldNotBeNull();
                handler.LastRequest!.Method.Method.ShouldBe("GET");
            }

            [Fact]
            public async Task SendAsync_ThrowsArgumentNullException_WhenMethodIsNull()
            {
                var handler = new TestHttpMessageHandler();
                var builder = CreateBuilder(handler);

                var exception = await Should
                    .ThrowAsync<ArgumentNullException>(() => builder.SendAsync((string)null!));

                exception.ParamName.ShouldBe("method");
            }

            [Fact]
            public async Task SendAsync_ThrowsArgumentException_WhenMethodIsEmpty()
            {
                var handler = new TestHttpMessageHandler();
                var builder = CreateBuilder(handler);

                var exception = await Should
                    .ThrowAsync<ArgumentException>(() => builder.SendAsync(string.Empty));

                exception.ParamName.ShouldBe("method");
            }
        }

        public class MethodAndCancellationTokenOverload
        {
            [Fact]
            public async Task SendAsync_UsesUppercaseHttpMethod_WhenCancellationTokenProvided()
            {
                var handler = new TestHttpMessageHandler();
                var builder = CreateBuilder(handler);
                using var cts = new CancellationTokenSource();

                var response = await builder.SendAsync("post", cts.Token);

                response.ShouldNotBeNull();
                handler.LastRequest.ShouldNotBeNull();
                handler.LastRequest!.Method.Method.ShouldBe("POST");
            }

            [Fact]
            public async Task SendAsync_ThrowsArgumentNullException_WhenMethodIsNull()
            {
                var handler = new TestHttpMessageHandler();
                var builder = CreateBuilder(handler);
                using var cts = new CancellationTokenSource();

                var exception = await Should
                    .ThrowAsync<ArgumentNullException>(() => builder.SendAsync((string)null!, cts.Token));

                exception.ParamName.ShouldBe("method");
            }

            [Fact]
            public async Task SendAsync_ThrowsArgumentException_WhenMethodIsEmpty()
            {
                var handler = new TestHttpMessageHandler();
                var builder = CreateBuilder(handler);
                using var cts = new CancellationTokenSource();

                var exception = await Should
                    .ThrowAsync<ArgumentException>(() => builder.SendAsync(string.Empty, cts.Token));

                exception.ParamName.ShouldBe("method");
            }
        }

        public class MethodAndCompletionOptionOverload
        {
            [Fact]
            public async Task SendAsync_UsesUppercaseHttpMethod_WhenCompletionOptionProvided()
            {
                var handler = new TestHttpMessageHandler();
                var builder = CreateBuilder(handler);

                var response = await builder.SendAsync("delete", HttpCompletionOption.ResponseHeadersRead);

                response.ShouldNotBeNull();
                handler.LastRequest.ShouldNotBeNull();
                handler.LastRequest!.Method.Method.ShouldBe("DELETE");
            }

            [Fact]
            public async Task SendAsync_ThrowsArgumentNullException_WhenMethodIsNull()
            {
                var handler = new TestHttpMessageHandler();
                var builder = CreateBuilder(handler);

                var exception = await Should
                    .ThrowAsync<ArgumentNullException>(() =>
                        builder.SendAsync((string)null!, HttpCompletionOption.ResponseContentRead));

                exception.ParamName.ShouldBe("method");
            }

            [Fact]
            public async Task SendAsync_ThrowsArgumentException_WhenMethodIsEmpty()
            {
                var handler = new TestHttpMessageHandler();
                var builder = CreateBuilder(handler);

                var exception = await Should
                    .ThrowAsync<ArgumentException>(() =>
                        builder.SendAsync(string.Empty, HttpCompletionOption.ResponseContentRead));

                exception.ParamName.ShouldBe("method");
            }
        }

        public class MethodCompletionOptionAndCancellationTokenOverload
        {
            [Fact]
            public async Task SendAsync_UsesUppercaseHttpMethod_WhenCompletionOptionAndCancellationTokenProvided()
            {
                var handler = new TestHttpMessageHandler();
                var builder = CreateBuilder(handler);
                using var cts = new CancellationTokenSource();

                var response = await builder.SendAsync(
                    "patch",
                    HttpCompletionOption.ResponseContentRead,
                    cts.Token);

                response.ShouldNotBeNull();
                handler.LastRequest.ShouldNotBeNull();
                handler.LastRequest!.Method.Method.ShouldBe("PATCH");
            }

            [Fact]
            public async Task SendAsync_ThrowsArgumentNullException_WhenMethodIsNull()
            {
                var handler = new TestHttpMessageHandler();
                var builder = CreateBuilder(handler);
                using var cts = new CancellationTokenSource();

                var exception = await Should
                    .ThrowAsync<ArgumentNullException>(() =>
                        builder.SendAsync((string)null!, HttpCompletionOption.ResponseContentRead, cts.Token));

                exception.ParamName.ShouldBe("method");
            }

            [Fact]
            public async Task SendAsync_ThrowsArgumentException_WhenMethodIsEmpty()
            {
                var handler = new TestHttpMessageHandler();
                var builder = CreateBuilder(handler);
                using var cts = new CancellationTokenSource();

                var exception = await Should
                    .ThrowAsync<ArgumentException>(() =>
                        builder.SendAsync(string.Empty, HttpCompletionOption.ResponseContentRead, cts.Token));

                exception.ParamName.ShouldBe("method");
            }
        }
    }

}
