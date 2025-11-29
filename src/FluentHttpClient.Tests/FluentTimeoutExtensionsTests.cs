namespace FluentHttpClient.Tests;

public class FluentTimeoutExtensionsTests
{
    private static HttpRequestBuilder CreateBuilder()
    {
        return new HttpRequestBuilder(new HttpClient(), "https://example.com");
    }

    public class ClearTimeout
    {
        [Fact]
        public void ClearTimeout_SetsTimeoutToNull()
        {
            var builder = CreateBuilder()
                .WithTimeout(60);

            builder.Timeout.ShouldNotBeNull();
            builder.Timeout.ShouldBe(TimeSpan.FromSeconds(60));

            builder.ClearTimeout();

            builder.Timeout.ShouldBeNull();
        }
    }

    public class WithTimeout_Int
    {
        [Fact]
        public void WithTimeout_SetsTimeoutFromSeconds_WhenValidSecondsProvided()
        {
            var builder = CreateBuilder()
                .WithTimeout(30);

            builder.Timeout.ShouldNotBeNull();
            builder.Timeout.ShouldBe(TimeSpan.FromSeconds(30));
        }

        [Fact]
        public void WithTimeout_ThrowsArgumentOutOfRangeException_WhenSecondsIsZero()
        {
            var builder = CreateBuilder();

            var exception = Should.Throw<ArgumentOutOfRangeException>(() => builder.WithTimeout(0));

            exception.ParamName.ShouldBe("seconds");
            exception.Message.ShouldContain(FluentTimeoutExtensions.MessageInvalidTimeout);
        }

        [Fact]
        public void WithTimeout_ThrowsArgumentOutOfRangeException_WhenSecondsIsNegative()
        {
            var builder = CreateBuilder();

            var exception = Should.Throw<ArgumentOutOfRangeException>(() => builder.WithTimeout(-1));

            exception.ParamName.ShouldBe("seconds");
            exception.Message.ShouldContain(FluentTimeoutExtensions.MessageInvalidTimeout);
        }

        [Fact]
        public void WithTimeout_OverwritesPreviousTimeout_WhenCalledMultipleTimes()
        {
            var builder = CreateBuilder();

            builder.WithTimeout(10);
            builder.Timeout.ShouldBe(TimeSpan.FromSeconds(10));

            builder.WithTimeout(20);
            builder.Timeout.ShouldBe(TimeSpan.FromSeconds(20));
        }
    }

    public class WithTimeout_TimeSpan
    {
        [Fact]
        public void WithTimeout_SetsTimeoutFromTimeSpan_WhenValidTimeSpanProvided()
        {
            var builder = CreateBuilder();
            var timeout = TimeSpan.FromMinutes(2);

            builder.WithTimeout(timeout);

            builder.Timeout.ShouldNotBeNull();
            builder.Timeout.ShouldBe(timeout);
        }

        [Fact]
        public void WithTimeout_ThrowsArgumentOutOfRangeException_WhenTimeSpanIsZero()
        {
            var builder = CreateBuilder();

            var exception = Should.Throw<ArgumentOutOfRangeException>(
                () => builder.WithTimeout(TimeSpan.Zero));

            exception.ParamName.ShouldBe("timeout");
            exception.Message.ShouldContain(FluentTimeoutExtensions.MessageInvalidTimeout);
        }

        [Fact]
        public void WithTimeout_ThrowsArgumentOutOfRangeException_WhenTimeSpanIsNegative()
        {
            var builder = CreateBuilder();

            var exception = Should.Throw<ArgumentOutOfRangeException>(
                () => builder.WithTimeout(TimeSpan.FromSeconds(-10)));

            exception.ParamName.ShouldBe("timeout");
            exception.Message.ShouldContain(FluentTimeoutExtensions.MessageInvalidTimeout);
        }

        [Fact]
        public void WithTimeout_OverwritesPreviousTimeout_WhenCalledMultipleTimesWithTimeSpan()
        {
            var builder = CreateBuilder();

            builder.WithTimeout(TimeSpan.FromSeconds(10));
            builder.WithTimeout(TimeSpan.FromMinutes(5));

            builder.Timeout.ShouldBe(TimeSpan.FromMinutes(5));
        }

        [Fact]
        public void WithTimeout_SupportsMillisecondPrecision()
        {
            var builder = CreateBuilder();
            var timeout = TimeSpan.FromMilliseconds(1500);

            builder.WithTimeout(timeout);

            builder.Timeout.ShouldBe(timeout);
            builder.Timeout!.Value.TotalMilliseconds.ShouldBe(1500);
        }
    }
}

