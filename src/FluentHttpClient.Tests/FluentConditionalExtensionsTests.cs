namespace FluentHttpClient.Tests;

public class FluentConditionalExtensionsTests
{
    public class EagerWhenTests
    {
        [Fact]
        public void When_InvokesConfigureImmediately_WhenConditionIsTrue()
        {
            var builder = CreateBuilder();
            var invoked = false;

            builder.When(true, b =>
            {
                invoked = true;
                b.WithHeader("X-Eager", "true");
            });

            invoked.ShouldBeTrue();
            builder.DeferredConfigurators.Count.ShouldBe(0);
        }

        [Fact]
        public void When_DoesNotInvokeConfigure_WhenConditionIsFalse()
        {
            var builder = CreateBuilder();
            var invoked = false;

            builder.When(false, b =>
            {
                invoked = true;
                b.WithHeader("X-Eager", "true");
            });

            invoked.ShouldBeFalse();
            builder.DeferredConfigurators.Count.ShouldBe(0);
        }

        [Fact]
        public async Task When_AppliesConfiguredChangesToBuiltRequest_WhenConditionIsTrue()
        {
            var builder = CreateBuilder();

            builder.When(true, b => b.WithHeader("X-Eager", "true"));

            var request = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);

            request.Headers.Contains("X-Eager").ShouldBeTrue();
            request.Headers.GetValues("X-Eager").ShouldContain("true");
        }

        [Fact]
        public async Task When_DoesNotAffectBuiltRequest_WhenConditionIsFalse()
        {
            var builder = CreateBuilder();

            builder.When(false, b => b.WithHeader("X-Eager", "true"));

            var request = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);

            request.Headers.Contains("X-Eager").ShouldBeFalse();
        }

        [Fact]
        public void When_ThrowsArgumentNullException_WhenConfigureIsNull_BoolOverload()
        {
            var builder = CreateBuilder();

            Should.Throw<ArgumentNullException>(() =>
                builder.When(true, null!));
        }
    }

    public class DeferredWhenTests
    {
        [Fact]
        public void When_AddsDeferredConfigurator_WhenPredicateIsProvided()
        {
            var builder = CreateBuilder();

            builder.When(() => true, b => b.WithHeader("X-Deferred", "true"));

            builder.DeferredConfigurators.Count.ShouldBe(1);
        }

        [Fact]
        public async Task When_ExecutesDeferredConfigurator_WhenPredicateReturnsTrue()
        {
            var builder = CreateBuilder();

            builder.When(() => true, b => b.WithHeader("X-Deferred", "true"));

            var request = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);

            request.Headers.Contains("X-Deferred").ShouldBeTrue();
            request.Headers.GetValues("X-Deferred").ShouldContain("true");
        }

        [Fact]
        public async Task When_DoesNotExecuteDeferredConfigurator_WhenPredicateReturnsFalse()
        {
            var builder = CreateBuilder();

            builder.When(() => false, b => b.WithHeader("X-Deferred", "true"));

            var request = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);

            request.Headers.Contains("X-Deferred").ShouldBeFalse();
        }

        [Fact]
        public async Task When_EvaluatesPredicateAtBuildTime_NotAtRegistration()
        {
            var builder = CreateBuilder();
            var evaluated = false;

            builder.When(
                () =>
                {
                    evaluated = true;
                    return true;
                },
                b => b.WithHeader("X-Deferred", "true"));

            evaluated.ShouldBeFalse();

            var request = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);

            evaluated.ShouldBeTrue();
            request.Headers.Contains("X-Deferred").ShouldBeTrue();
        }

        [Fact]
        public async Task When_EvaluatesPredicateOnEveryBuild_WhenBuildingMultipleRequests()
        {
            var builder = CreateBuilder();
            var evaluationCount = 0;

            builder.When(
                () =>
                {
                    evaluationCount++;
                    return true;
                },
                b => b.WithHeader("X-Deferred", "true"));

            var first = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);
            var second = await builder.BuildRequest(HttpMethod.Get, CancellationToken.None);

            evaluationCount.ShouldBe(2);

            first.Headers.Contains("X-Deferred").ShouldBeTrue();
            second.Headers.Contains("X-Deferred").ShouldBeTrue();
        }

        [Fact]
        public void When_ThrowsArgumentNullException_WhenPredicateIsNull()
        {
            var builder = CreateBuilder();

            Should.Throw<ArgumentNullException>(() =>
                builder.When(null!, b => b.WithHeader("X-Deferred", "true")));
        }

        [Fact]
        public void When_ThrowsArgumentNullException_WhenConfigureIsNull_PredicateOverload()
        {
            var builder = CreateBuilder();

            Should.Throw<ArgumentNullException>(() =>
                builder.When(() => true, null!));
        }
    }

    private static HttpRequestBuilder CreateBuilder()
    {
        var client = new HttpClient();
        return new HttpRequestBuilder(client, "https://example.com/");
    }
}
