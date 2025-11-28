namespace FluentHttpClient.Tests;

public class FluentHeaderExtensionsTests
{
    public class WithHeader_StringValueTests
    {
        [Fact]
        public void WithHeader_AddsConfigurator_WhenCalled()
        {
            var builder = CreateBuilder();

            var result = builder.WithHeader("X-Test", "value");

            result.ShouldBeSameAs(builder);
            builder.InternalHeaders.Count.ShouldBe(1);
        }

        [Fact]
        public void WithHeader_AddsHeaderToRequest_WhenConfiguratorInvoked()
        {
            var builder = CreateBuilder();

            builder.WithHeader("X-Test", "value");

            builder.InternalHeaders.ShouldContainKey("X-Test");
            builder.InternalHeaders["X-Test"].ShouldBe(new[] { "value" });
        }

        [Fact]
        public void WithHeader_ThrowsArgumentNullException_WhenKeyIsNull()
        {
            var builder = CreateBuilder();

            var ex = Should.Throw<ArgumentNullException>(() =>
                builder.WithHeader(null!, "value"));

            ex.ParamName.ShouldBe("key");
        }

        [Theory]
        [InlineData("Host")]
        [InlineData("Content-Length")]
        [InlineData("Transfer-Encoding")]
        public void WithHeader_ThrowsArgumentException_WhenKeyIsReserved(string key)
        {
            var builder = CreateBuilder();

            var ex = Should.Throw<ArgumentException>(() =>
                builder.WithHeader(key, "value"));

            ex.ParamName.ShouldBe("key");
        }

        [Fact]
        public void WithHeader_ThrowsArgumentNullException_WhenValueIsNull()
        {
            var builder = CreateBuilder();

            var ex = Should.Throw<ArgumentNullException>(() =>
                builder.WithHeader("X-Test", (string)null!));

            ex.ParamName.ShouldBe("value");
        }
    }

    public class WithHeader_MultiValueTests
    {
        [Fact]
        public void WithHeader_AddsConfigurator_WhenCalled()
        {
            var builder = CreateBuilder();
            var values = new[] { "one", "two" };

            var result = builder.WithHeader("X-Multi", values);

            result.ShouldBeSameAs(builder);
            builder.InternalHeaders.Count.ShouldBe(1);
        }

        [Fact]
        public void WithHeader_AddsMultipleValuesToRequest_WhenConfiguratorInvoked()
        {
            var builder = CreateBuilder();
            var values = new[] { "one", "two" };

            builder.WithHeader("X-Multi", values);

            builder.InternalHeaders.ShouldContainKey("X-Multi");
            builder.InternalHeaders["X-Multi"].ShouldBe(values);
        }

        [Fact]
        public void WithHeader_ThrowsArgumentNullException_WhenKeyIsNull()
        {
            var builder = CreateBuilder();
            var values = new[] { "one", "two" };

            var ex = Should.Throw<ArgumentNullException>(() =>
                builder.WithHeader(null!, values));

            ex.ParamName.ShouldBe("key");
        }

        [Theory]
        [InlineData("Host")]
        [InlineData("Content-Length")]
        [InlineData("Transfer-Encoding")]
        public void WithHeader_ThrowsArgumentException_WhenKeyIsReserved(string key)
        {
            var builder = CreateBuilder();
            var values = new[] { "one", "two" };

            var ex = Should.Throw<ArgumentException>(() =>
                builder.WithHeader(key, values));

            ex.ParamName.ShouldBe("key");
        }

        [Fact]
        public void WithHeader_ThrowsArgumentNullException_WhenValuesIsNull()
        {
            var builder = CreateBuilder();

            var ex = Should.Throw<ArgumentNullException>(() =>
                builder.WithHeader("X-Multi", (IEnumerable<string>)null!));

            ex.ParamName.ShouldBe("values");
        }
    }

    public class WithHeaders_SingleValueTests
    {
        [Fact]
        public void WithHeaders_AddsConfigurator_WhenCalled()
        {
            var builder = CreateBuilder();
            var headers = new[]
            {
                new KeyValuePair<string, string>("X-One", "1"),
                new KeyValuePair<string, string>("X-Two", "2")
            };

            var result = builder.WithHeaders(headers);

            result.ShouldBeSameAs(builder);
            builder.InternalHeaders.Count.ShouldBe(2);
        }

        [Fact]
        public void WithHeaders_AddsAllHeadersToRequest_WhenConfiguratorInvoked()
        {
            var builder = CreateBuilder();
            var headers = new[]
            {
                new KeyValuePair<string, string>("X-One", "1"),
                new KeyValuePair<string, string>("X-Two", "2")
            };

            builder.WithHeaders(headers);

            builder.InternalHeaders.ShouldContainKey("X-One");
            builder.InternalHeaders["X-One"].ShouldBe(new[] { "1" });

            builder.InternalHeaders.ShouldContainKey("X-Two");
            builder.InternalHeaders["X-Two"].ShouldBe(new[] { "2" });
        }

        [Fact]
        public void WithHeaders_ThrowsArgumentNullException_WhenHeadersIsNull()
        {
            var builder = CreateBuilder();

            var ex = Should.Throw<ArgumentNullException>(() =>
                builder.WithHeaders((IEnumerable<KeyValuePair<string, string>>)null!));

            ex.ParamName.ShouldBe("headers");
        }

        [Fact]
        public void WithHeaders_ThrowsArgumentException_WhenHeaderKeyIsNull()
        {
            var builder = CreateBuilder();
            var headers = new[]
            {
                new KeyValuePair<string, string>(null!, "1")
            };

            var ex = Should.Throw<ArgumentException>(() =>
                builder.WithHeaders(headers));

            ex.ParamName.ShouldBe("key");
        }

        [Theory]
        [InlineData("Host")]
        [InlineData("Content-Length")]
        [InlineData("Transfer-Encoding")]
        public void WithHeader_ThrowsArgumentException_WhenKeyIsReserved(string key)
        {
            var builder = CreateBuilder();
            var headers = new[]
            {
                new KeyValuePair<string, string>(key, "1")
            };

            var ex = Should.Throw<ArgumentException>(() =>
                builder.WithHeaders(headers));

            ex.ParamName.ShouldBe("key");
        }

        [Fact]
        public void WithHeaders_ThrowsArgumentException_WhenHeaderValueIsNull()
        {
            var builder = CreateBuilder();
            var headers = new[]
            {
                new KeyValuePair<string, string>("X-One", null!)
            };

            var ex = Should.Throw<ArgumentException>(() =>
                builder.WithHeaders(headers));

            ex.ParamName.ShouldBe("value");
        }
    }

    public class WithHeaders_MultiValueTests
    {
        [Fact]
        public void WithHeaders_AddsConfigurator_WhenCalled()
        {
            var builder = CreateBuilder();
            var headers = new[]
            {
                new KeyValuePair<string, IEnumerable<string>>("X-One", new[] { "1", "1b" }),
                new KeyValuePair<string, IEnumerable<string>>("X-Two", new[] { "2" })
            };

            var result = builder.WithHeaders(headers);

            result.ShouldBeSameAs(builder);
            builder.InternalHeaders.Count.ShouldBe(2);
        }

        [Fact]
        public void WithHeaders_AddsAllHeadersAndValuesToRequest_WhenConfiguratorInvoked()
        {
            var builder = CreateBuilder();
            var headers = new[]
            {
                new KeyValuePair<string, IEnumerable<string>>("X-One", new[] { "1", "1b" }),
                new KeyValuePair<string, IEnumerable<string>>("X-Two", new[] { "2" })
            };

            builder.WithHeaders(headers);

            builder.InternalHeaders.ShouldContainKey("X-One");
            builder.InternalHeaders["X-One"].ShouldBe(new[] { "1", "1b" });

            builder.InternalHeaders.ShouldContainKey("X-Two");
            builder.InternalHeaders["X-Two"].ShouldBe(new[] { "2" });
        }

        [Fact]
        public void WithHeaders_ThrowsArgumentNullException_WhenHeadersIsNull()
        {
            var builder = CreateBuilder();

            var ex = Should.Throw<ArgumentNullException>(() =>
                builder.WithHeaders((IEnumerable<KeyValuePair<string, IEnumerable<string>>>)null!));

            ex.ParamName.ShouldBe("headers");
        }

        [Theory]
        [InlineData("Host")]
        [InlineData("Content-Length")]
        [InlineData("Transfer-Encoding")]
        public void WithHeader_ThrowsArgumentException_WhenKeyIsReserved(string key)
        {
            var builder = CreateBuilder();
            var headers = new[]
            {
                new KeyValuePair<string, IEnumerable<string>>(key, new[] { "1" })
            };

            var ex = Should.Throw<ArgumentException>(() =>
                builder.WithHeaders(headers));

            ex.ParamName.ShouldBe("key");
        }

        [Fact]
        public void WithHeaders_ThrowsArgumentException_WhenHeaderKeyIsNull()
        {
            var builder = CreateBuilder();
            var headers = new[]
            {
                new KeyValuePair<string, IEnumerable<string>>(null!, new[] { "1" })
            };

            var ex = Should.Throw<ArgumentException>(() =>
                builder.WithHeaders(headers));

            ex.ParamName.ShouldBe("key");
        }

        [Fact]
        public void WithHeaders_ThrowsArgumentException_WhenHeaderValuesIsNull()
        {
            var builder = CreateBuilder();
            var headers = new[]
            {
                new KeyValuePair<string, IEnumerable<string>>("X-One", null!)
            };

            var ex = Should.Throw<ArgumentException>(() =>
                builder.WithHeaders(headers));

            ex.ParamName.ShouldBe("values");
        }
    }

    public class ConfigureHeadersTests
    {
        [Fact]
        public void ConfigureHeaders_AddsConfiguratorToList_WhenCalled()
        {
            var builder = CreateBuilder();

            var result = builder.ConfigureHeaders(headers =>
                headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "token"));

            result.ShouldBeSameAs(builder);
            builder.HeaderConfigurators.Count.ShouldBe(1);
        }

        [Fact]
        public async Task ConfigureHeaders_AppliesTypedHeaderCorrectly_WhenRequestIsBuilt()
        {
            var builder = CreateBuilder();

            builder.ConfigureHeaders(headers =>
                headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "test-token"));

            var request = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);

            request.Headers.Authorization.ShouldNotBeNull();
            request.Headers.Authorization!.Scheme.ShouldBe("Bearer");
            request.Headers.Authorization.Parameter.ShouldBe("test-token");
        }

        [Fact]
        public async Task ConfigureHeaders_AppliesCacheControlHeader_WhenRequestIsBuilt()
        {
            var builder = CreateBuilder();

            builder.ConfigureHeaders(headers =>
            {
                headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
                {
                    NoCache = true,
                    NoStore = true,
                    MaxAge = TimeSpan.FromSeconds(30)
                };
            });

            var request = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);

            request.Headers.CacheControl.ShouldNotBeNull();
            request.Headers.CacheControl!.NoCache.ShouldBeTrue();
            request.Headers.CacheControl.NoStore.ShouldBeTrue();
            request.Headers.CacheControl.MaxAge.ShouldBe(TimeSpan.FromSeconds(30));
        }

        [Fact]
        public async Task ConfigureHeaders_AppliesMultipleAcceptHeaders_WhenRequestIsBuilt()
        {
            var builder = CreateBuilder();

            builder.ConfigureHeaders(headers =>
            {
                headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml", 0.9));
            });

            var request = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);

            request.Headers.Accept.Count.ShouldBe(2);
            request.Headers.Accept.First().MediaType.ShouldBe("application/json");
            request.Headers.Accept.First().Quality.ShouldBeNull();
            request.Headers.Accept.Last().MediaType.ShouldBe("application/xml");
            request.Headers.Accept.Last().Quality.ShouldBe(0.9);
        }

        [Fact]
        public async Task ConfigureHeaders_AccumulatesMultipleConfigurators_WhenCalledMultipleTimes()
        {
            var builder = CreateBuilder();

            builder.ConfigureHeaders(headers =>
                    headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "token"))
                .ConfigureHeaders(headers =>
                    headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue { NoCache = true });

            var request = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);

            builder.HeaderConfigurators.Count.ShouldBe(2);
            request.Headers.Authorization.ShouldNotBeNull();
            request.Headers.Authorization!.Scheme.ShouldBe("Bearer");
            request.Headers.CacheControl.ShouldNotBeNull();
            request.Headers.CacheControl!.NoCache.ShouldBeTrue();
        }

        [Fact]
        public async Task ConfigureHeaders_AppliesIfModifiedSinceHeader_WhenRequestIsBuilt()
        {
            var builder = CreateBuilder();
            var testDate = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);

            builder.ConfigureHeaders(headers =>
                headers.IfModifiedSince = testDate);

            var request = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);

            request.Headers.IfModifiedSince.ShouldBe(testDate);
        }

        [Fact]
        public async Task ConfigureHeaders_AppliesIfNoneMatchHeader_WhenRequestIsBuilt()
        {
            var builder = CreateBuilder();

            builder.ConfigureHeaders(headers =>
                headers.IfNoneMatch.Add(new System.Net.Http.Headers.EntityTagHeaderValue("\"12345\"")));

            var request = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);

            request.Headers.IfNoneMatch.Count.ShouldBe(1);
            request.Headers.IfNoneMatch.First().Tag.ShouldBe("\"12345\"");
        }

        [Fact]
        public void ConfigureHeaders_ThrowsArgumentNullException_WhenConfigureIsNull()
        {
            var builder = CreateBuilder();

            var ex = Should.Throw<ArgumentNullException>(() =>
                builder.ConfigureHeaders(null!));

            ex.ParamName.ShouldBe("configure");
        }

        [Fact]
        public async Task ConfigureHeaders_WorksWithOtherFluentMethods_WhenChained()
        {
            var builder = CreateBuilder();

            builder.WithHeader("X-Custom", "value")
                .ConfigureHeaders(headers =>
                    headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "token"))
                .WithHeader("X-Another", "value2");

            var request = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);

            // Verify InternalHeaders were applied
            builder.InternalHeaders.ShouldContainKey("X-Custom");
            builder.InternalHeaders.ShouldContainKey("X-Another");

            // Verify typed header was applied
            request.Headers.Authorization.ShouldNotBeNull();
            request.Headers.Authorization!.Scheme.ShouldBe("Bearer");
        }
    }

    private static HttpRequestBuilder CreateBuilder() =>
        new HttpRequestBuilder(new HttpClient(), "https://example.com");
}
