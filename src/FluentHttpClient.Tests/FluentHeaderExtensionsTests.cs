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
            builder.HeaderConfigurators.Count.ShouldBe(1);
        }

        [Fact]
        public void WithHeader_AddsHeaderToRequest_WhenConfiguratorInvoked()
        {
            var builder = CreateBuilder();

            builder.WithHeader("X-Test", "value");
            var configurator = builder.HeaderConfigurators.Single();

            var message = new HttpRequestMessage(HttpMethod.Get, "https://example.com/resource");
            configurator(message.Headers);

            message.Headers.Contains("X-Test").ShouldBeTrue();
            message.Headers.GetValues("X-Test").Single().ShouldBe("value");
        }

        [Fact]
        public void WithHeader_ThrowsArgumentNullException_WhenKeyIsNull()
        {
            var builder = CreateBuilder();

            var ex = Should.Throw<ArgumentNullException>(() =>
                builder.WithHeader(null!, "value"));

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
            builder.HeaderConfigurators.Count.ShouldBe(1);
        }

        [Fact]
        public void WithHeader_AddsMultipleValuesToRequest_WhenConfiguratorInvoked()
        {
            var builder = CreateBuilder();
            var values = new[] { "one", "two" };

            builder.WithHeader("X-Multi", values);
            var configurator = builder.HeaderConfigurators.Single();

            var message = new HttpRequestMessage(HttpMethod.Get, "https://example.com/resource");
            configurator(message.Headers);

            message.Headers.Contains("X-Multi").ShouldBeTrue();
            message.Headers.GetValues("X-Multi").ShouldBe(values);
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
            builder.HeaderConfigurators.Count.ShouldBe(1);
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
            var configurator = builder.HeaderConfigurators.Single();

            var message = new HttpRequestMessage(HttpMethod.Get, "https://example.com/resource");
            configurator(message.Headers);

            message.Headers.Contains("X-One").ShouldBeTrue();
            message.Headers.GetValues("X-One").Single().ShouldBe("1");

            message.Headers.Contains("X-Two").ShouldBeTrue();
            message.Headers.GetValues("X-Two").Single().ShouldBe("2");
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

            builder.WithHeaders(headers);
            var configurator = builder.HeaderConfigurators.Single();

            var message = new HttpRequestMessage(HttpMethod.Get, "https://example.com/resource");
            var ex = Should.Throw<ArgumentException>(() =>
                configurator(message.Headers));

            ex.ParamName.ShouldBe("headers");
            ex.Message.ShouldStartWith("Header name cannot be null.");
        }

        [Fact]
        public void WithHeaders_ThrowsArgumentException_WhenHeaderValueIsNull()
        {
            var builder = CreateBuilder();
            var headers = new[]
            {
                new KeyValuePair<string, string>("X-One", null!)
            };

            builder.WithHeaders(headers);
            var configurator = builder.HeaderConfigurators.Single();

            var message = new HttpRequestMessage(HttpMethod.Get, "https://example.com/resource");
            var ex = Should.Throw<ArgumentException>(() =>
                configurator(message.Headers));

            ex.ParamName.ShouldBe("headers");
            ex.Message.ShouldStartWith("Header values for 'X-One' cannot be null.");
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
            builder.HeaderConfigurators.Count.ShouldBe(1);
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
            var configurator = builder.HeaderConfigurators.Single();

            var message = new HttpRequestMessage(HttpMethod.Get, "https://example.com/resource");
            configurator(message.Headers);

            message.Headers.Contains("X-One").ShouldBeTrue();
            message.Headers.GetValues("X-One").ShouldBe(new[] { "1", "1b" });

            message.Headers.Contains("X-Two").ShouldBeTrue();
            message.Headers.GetValues("X-Two").Single().ShouldBe("2");
        }

        [Fact]
        public void WithHeaders_ThrowsArgumentNullException_WhenHeadersIsNull()
        {
            var builder = CreateBuilder();

            var ex = Should.Throw<ArgumentNullException>(() =>
                builder.WithHeaders((IEnumerable<KeyValuePair<string, IEnumerable<string>>>)null!));

            ex.ParamName.ShouldBe("headers");
        }

        [Fact]
        public void WithHeaders_ThrowsArgumentException_WhenHeaderKeyIsNull()
        {
            var builder = CreateBuilder();
            var headers = new[]
            {
                new KeyValuePair<string, IEnumerable<string>>(null!, new[] { "1" })
            };

            builder.WithHeaders(headers);
            var configurator = builder.HeaderConfigurators.Single();

            var message = new HttpRequestMessage(HttpMethod.Get, "https://example.com/resource");
            var ex = Should.Throw<ArgumentException>(() =>
                configurator(message.Headers));

            ex.ParamName.ShouldBe("headers");
            ex.Message.ShouldStartWith("Header name cannot be null.");
        }

        [Fact]
        public void WithHeaders_ThrowsArgumentException_WhenHeaderValuesIsNull()
        {
            var builder = CreateBuilder();
            var headers = new[]
            {
                new KeyValuePair<string, IEnumerable<string>>("X-One", null!)
            };

            builder.WithHeaders(headers);
            var configurator = builder.HeaderConfigurators.Single();

            var message = new HttpRequestMessage(HttpMethod.Get, "https://example.com/resource");
            var ex = Should.Throw<ArgumentException>(() =>
                configurator(message.Headers));

            ex.ParamName.ShouldBe("headers");
            ex.Message.ShouldStartWith("Header values for 'X-One' cannot be null.");
        }
    }

    private static HttpRequestBuilder CreateBuilder() =>
        new HttpRequestBuilder(new HttpClient(), "https://example.com");
}
