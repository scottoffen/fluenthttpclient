namespace FluentHttpClient.Tests;

public class FluentVersionExtensionsTests
{
    private static HttpRequestBuilder CreateBuilder()
    {
        return new HttpRequestBuilder(new HttpClient(), "https://example.com");
    }

    public class UsingVersion_String
    {
        [Fact]
        public void UsingVersion_SetsVersion_WhenValidStringProvided()
        {
            var builder = CreateBuilder();

            var result = builder.UsingVersion("2.0");

            result.ShouldBe(builder);
            builder.Version.ShouldBe(new Version(2, 0));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void UsingVersion_ThrowsArgumentException_WhenVersionStringIsNullOrWhitespace(string? version)
        {
            var builder = CreateBuilder();

            var exception = Should.Throw<ArgumentException>(() => builder.UsingVersion(version!));

            exception.ParamName.ShouldBe("version");
        }

        [Fact]
        public void UsingVersion_ThrowsArgumentException_WhenVersionStringIsInvalid()
        {
            var builder = CreateBuilder();

            var exception = Should.Throw<ArgumentException>(() => builder.UsingVersion("HTTP/1.1"));

            exception.ParamName.ShouldBe("version");
            exception.Message.ShouldContain("valid version string");
        }

        [Fact]
        public void UsingVersion_DoesNotChangeVersionPolicy_WhenOnlyVersionIsSet()
        {
            var builder = CreateBuilder();
            builder.VersionPolicy = HttpVersionPolicy.RequestVersionOrLower;

            builder.UsingVersion("1.1");

            builder.Version.ShouldBe(new Version(1, 1));
            builder.VersionPolicy.ShouldBe(HttpVersionPolicy.RequestVersionOrLower);
        }
    }

    public class UsingVersion_MajorMinor
    {
        [Fact]
        public void UsingVersion_SetsVersion_WhenMajorAndMinorSpecified()
        {
            var builder = CreateBuilder();

            var result = builder.UsingVersion(3, 1);

            result.ShouldBe(builder);
            builder.Version.ShouldBe(new Version(3, 1));
        }
    }

    public class UsingVersion_Version
    {
        [Fact]
        public void UsingVersion_SetsVersion_WhenVersionInstanceProvided()
        {
            var builder = CreateBuilder();
            var version = new Version(1, 0);

            var result = builder.UsingVersion(version);

            result.ShouldBe(builder);
            builder.Version.ShouldBe(version);
        }

        [Fact]
        public void UsingVersion_ThrowsArgumentNullException_WhenVersionIsNull()
        {
            var builder = CreateBuilder();

            var exception = Should.Throw<ArgumentNullException>(() => builder.UsingVersion((Version)null!));

            exception.ParamName.ShouldBe("version");
        }
    }

    public class UsingVersion_StringAndPolicy
    {
        [Fact]
        public void UsingVersion_SetsVersionAndPolicy_WhenStringAndPolicyProvided()
        {
            var builder = CreateBuilder();

            var result = builder.UsingVersion("2.0", HttpVersionPolicy.RequestVersionOrHigher);

            result.ShouldBe(builder);
            builder.Version.ShouldBe(new Version(2, 0));
            builder.VersionPolicy.ShouldBe(HttpVersionPolicy.RequestVersionOrHigher);
        }

        [Fact]
        public void UsingVersion_ThrowsArgumentException_WhenStringIsInvalid_EvenWhenPolicyProvided()
        {
            var builder = CreateBuilder();

            var exception = Should.Throw<ArgumentException>(
                () => builder.UsingVersion("HTTP/1.1", HttpVersionPolicy.RequestVersionOrLower));

            exception.ParamName.ShouldBe("version");
        }
    }

    public class UsingVersion_VersionAndPolicy
    {
        [Fact]
        public void UsingVersion_SetsVersionAndPolicy_WhenVersionAndPolicyProvided()
        {
            var builder = CreateBuilder();
            var version = new Version(1, 1);

            var result = builder.UsingVersion(version, HttpVersionPolicy.RequestVersionExact);

            result.ShouldBe(builder);
            builder.Version.ShouldBe(version);
            builder.VersionPolicy.ShouldBe(HttpVersionPolicy.RequestVersionExact);
        }

        [Fact]
        public void UsingVersion_ThrowsArgumentNullException_WhenVersionIsNullAndPolicyProvided()
        {
            var builder = CreateBuilder();

            var exception = Should.Throw<ArgumentNullException>(
                () => builder.UsingVersion((Version)null!, HttpVersionPolicy.RequestVersionOrLower));

            exception.ParamName.ShouldBe("version");
        }
    }

    public class UsingVersionPolicy_Only
    {
        [Fact]
        public void UsingVersionPolicy_SetsVersionPolicy_WhenPolicyProvided()
        {
            var builder = CreateBuilder();
            var originalVersion = new Version(1, 1);
            builder.Version = originalVersion;

            var result = builder.UsingVersionPolicy(HttpVersionPolicy.RequestVersionOrHigher);

            result.ShouldBe(builder);
            builder.VersionPolicy.ShouldBe(HttpVersionPolicy.RequestVersionOrHigher);
            builder.Version.ShouldBe(originalVersion);
        }
    }
}
