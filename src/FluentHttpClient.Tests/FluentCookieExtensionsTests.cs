namespace FluentHttpClient.Tests;

public class FluentCookieExtensionsTests
{
    public class WithCookie
    {
        [Fact]
        public void AddsCookie_WhenNameAndValueProvided()
        {
            var builder = CreateBuilder();

            builder.WithCookie("session-id", "abc123");

            builder.Cookies.ShouldContainKeyAndValue("session-id", "abc123");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ThrowsArgumentException_WhenNameIsNullOrWhitespace(string? name)
        {
            var builder = CreateBuilder();

            var exception = Should.Throw<ArgumentException>(() => builder.WithCookie(name!, "value"));

            exception.ParamName.ShouldBe("name");
        }

        [Fact]
        public void UsesEmptyString_WhenValueIsNull()
        {
            var builder = CreateBuilder();

            builder.WithCookie("session-id", null!);

            builder.Cookies.ShouldContainKey("session-id");
            builder.Cookies["session-id"].ShouldBe(string.Empty);
        }

        [Fact]
        public void OverwritesExistingCookie_WhenNameAlreadyExists()
        {
            var builder = CreateBuilder();
            builder.Cookies["session-id"] = "original";

            builder.WithCookie("session-id", "updated");

            builder.Cookies["session-id"].ShouldBe("updated");
        }
    }

    public class WithCookies
    {
        [Fact]
        public void ThrowsArgumentNullException_WhenCookiesIsNull()
        {
            var builder = CreateBuilder();

            var exception = Should.Throw<ArgumentNullException>(() => builder.WithCookies(null!));

            exception.ParamName.ShouldBe("cookies");
        }

        [Fact]
        public void AddsAllCookies_WhenValidCollectionProvided()
        {
            var builder = CreateBuilder();
            var cookies = new Dictionary<string, string>
            {
                ["session-id"] = "abc123",
                ["theme"] = "dark"
            };

            builder.WithCookies(cookies);

            builder.Cookies.Count.ShouldBe(2);
            builder.Cookies.ShouldContainKeyAndValue("session-id", "abc123");
            builder.Cookies.ShouldContainKeyAndValue("theme", "dark");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ThrowsArgumentException_WhenCookieNameIsNullOrWhitespace(string? name)
        {
            var builder = CreateBuilder();
            var cookies = new[]
            {
                new KeyValuePair<string, string>("valid", "ok"),
                new KeyValuePair<string, string>(name!, "value")
            };

            var exception = Should.Throw<ArgumentException>(() => builder.WithCookies(cookies));

            exception.ParamName.ShouldBe("cookies");
        }

        [Fact]
        public void OverwritesExistingCookies_WhenNamesOverlap()
        {
            var builder = CreateBuilder();
            builder.Cookies["session-id"] = "original";

            var cookies = new Dictionary<string, string>
            {
                ["session-id"] = "updated",
                ["new-cookie"] = "value"
            };

            builder.WithCookies(cookies);

            builder.Cookies["session-id"].ShouldBe("updated");
            builder.Cookies["new-cookie"].ShouldBe("value");
        }

        [Fact]
        public void UsesEmptyStringForNullValues_WhenAddingMultipleCookies()
        {
            var builder = CreateBuilder();
            var cookies = new[]
            {
                new KeyValuePair<string, string>("session-id", null!),
                new KeyValuePair<string, string>("theme", "dark")
            };

            builder.WithCookies(cookies);

            builder.Cookies["session-id"].ShouldBe(string.Empty);
            builder.Cookies["theme"].ShouldBe("dark");
        }
    }

    private static HttpRequestBuilder CreateBuilder() =>
        new HttpRequestBuilder(new HttpClient(), "https://example.com");
}
