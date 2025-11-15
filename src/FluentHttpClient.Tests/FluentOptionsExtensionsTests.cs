namespace FluentHttpClient.Tests;

public class FluentOptionsExtensionsTests
{
    public class ConfigureOptionsTests
    {
        [Fact]
        public void ConfigureOptions_AddsConfiguratorAndReturnsBuilder_WhenArgumentsAreValid()
        {
            var client = new HttpClient();
            var builder = new HttpRequestBuilder(client, "https://example.com");
            var key = new HttpRequestOptionsKey<string>("trace-id");
            var value = "abc123";
            var invoked = false;

            var result = builder.ConfigureOptions(options =>
            {
                invoked = true;
                options.Set(key, value);
            });

            result.ShouldBeSameAs(builder);
            builder.OptionConfigurators.Count.ShouldBe(1);

            var options = new HttpRequestOptions();
            builder.OptionConfigurators[0](options);

            invoked.ShouldBeTrue();
            options.TryGetValue(key, out string? actual).ShouldBeTrue();
            actual.ShouldBe(value);
        }

        [Fact]
        public void ConfigureOptions_ThrowsArgumentNullException_WhenActionIsNull()
        {
            var client = new HttpClient();
            var builder = new HttpRequestBuilder(client, "https://example.com");
            Action<HttpRequestOptions>? action = null;

            var exception = Should.Throw<ArgumentNullException>(
                () => builder.ConfigureOptions(action!));

            exception.ParamName.ShouldBe("action");
        }
    }

    public class WithOptionTests
    {
        [Fact]
        public void WithOption_AddsConfiguratorAndReturnsBuilder_WhenCalled()
        {
            var client = new HttpClient();
            var builder = new HttpRequestBuilder(client, "https://example.com");
            var key = new HttpRequestOptionsKey<string>("correlation-id");
            var value = "xyz789";

            var result = builder.WithOption(key, value);

            result.ShouldBeSameAs(builder);
            builder.OptionConfigurators.Count.ShouldBe(1);

            var options = new HttpRequestOptions();
            builder.OptionConfigurators[0](options);

            options.TryGetValue(key, out string? actual).ShouldBeTrue();
            actual.ShouldBe(value);
        }

        [Fact]
        public void WithOption_UsesGenericTypeOfKey_WhenStoringValue()
        {
            var client = new HttpClient();
            var builder = new HttpRequestBuilder(client, "https://example.com");
            var key = new HttpRequestOptionsKey<int>("retry-count");
            var value = 3;

            builder.WithOption(key, value);

            var options = new HttpRequestOptions();
            builder.OptionConfigurators[0](options);

            options.TryGetValue(key, out int actual).ShouldBeTrue();
            actual.ShouldBe(value);
        }

        [Fact]
        public void WithOption_ConfiguratorOverridesValue_WhenCalledMultipleTimesWithSameKey()
        {
            var client = new HttpClient();
            var builder = new HttpRequestBuilder(client, "https://example.com");
            var key = new HttpRequestOptionsKey<string>("feature-flag");

            builder.WithOption(key, "first");
            builder.WithOption(key, "second");

            builder.OptionConfigurators.Count.ShouldBe(2);

            var options = new HttpRequestOptions();
            foreach (var configurator in builder.OptionConfigurators)
            {
                configurator(options);
            }

            options.TryGetValue(key, out string? actual).ShouldBeTrue();
            actual.ShouldBe("second");
        }
    }
}
