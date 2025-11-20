using System.Net;

namespace FluentHttpClient.Tests;

public class FluentResponseHandlerExtensionsTests
{
    private static Task<HttpResponseMessage> CreateResponseTask(HttpStatusCode statusCode)
    {
        var response = new HttpResponseMessage(statusCode);
        return Task.FromResult(response);
    }

    public class When_SyncHandler
    {
        [Fact]
        public async Task InvokesHandler_WhenPredicateReturnsTrue()
        {
            var called = false;

            var responseTask = CreateResponseTask(HttpStatusCode.OK)
                .When(_ => true, _ => called = true);

            var response = await responseTask;

            called.ShouldBeTrue();
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DoesNotInvokeHandler_WhenPredicateReturnsFalse()
        {
            var called = false;

            var responseTask = CreateResponseTask(HttpStatusCode.OK)
                .When(_ => false, _ => called = true);

            var response = await responseTask;

            called.ShouldBeFalse();
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ReturnsSameResponseInstance()
        {
            var original = new HttpResponseMessage(HttpStatusCode.OK);
            var task = Task.FromResult(original);

            var result = await task.When(_ => true, _ => { });

            result.ShouldBeSameAs(original);
        }

        [Fact]
        public async Task ThrowsArgumentNullException_WhenPredicateIsNull()
        {
            var task = CreateResponseTask(HttpStatusCode.OK);

            await Should.ThrowAsync<ArgumentNullException>(async () =>
            {
                await task.When(null!, _ => { });
            });
        }

        [Fact]
        public async Task ThrowsArgumentNullException_WhenHandlerIsNull()
        {
            var task = CreateResponseTask(HttpStatusCode.OK);

            await Should.ThrowAsync<ArgumentNullException>(async () =>
            {
                await task.When(_ => true, (Action<HttpResponseMessage>)null!);
            });
        }

        [Fact]
        public async Task PropagatesException_ThrownByHandler()
        {
            var task = CreateResponseTask(HttpStatusCode.OK);

            await Should.ThrowAsync<InvalidOperationException>(async () =>
            {
                await task.When(_ => true, _ => throw new InvalidOperationException("boom"));
            });
        }
    }

    public class When_AsyncHandler
    {
        [Fact]
        public async Task InvokesAsyncHandler_WhenPredicateReturnsTrue()
        {
            var called = false;

            var responseTask = CreateResponseTask(HttpStatusCode.OK)
                .When(_ => true, async _ =>
                {
                    await Task.Yield();
                    called = true;
                });

            var response = await responseTask;

            called.ShouldBeTrue();
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DoesNotInvokeAsyncHandler_WhenPredicateReturnsFalse()
        {
            var called = false;

            var responseTask = CreateResponseTask(HttpStatusCode.OK)
                .When(_ => false, async _ =>
                {
                    await Task.Yield();
                    called = true;
                });

            var response = await responseTask;

            called.ShouldBeFalse();
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ReturnsSameResponseInstance()
        {
            var original = new HttpResponseMessage(HttpStatusCode.OK);
            var task = Task.FromResult(original);

            var result = await task.When(_ => true, _ => Task.CompletedTask);

            result.ShouldBeSameAs(original);
        }

        [Fact]
        public async Task ThrowsArgumentNullException_WhenPredicateIsNull()
        {
            var task = CreateResponseTask(HttpStatusCode.OK);

            await Should.ThrowAsync<ArgumentNullException>(async () =>
            {
                await task.When(null!, _ => Task.CompletedTask);
            });
        }

        [Fact]
        public async Task ThrowsArgumentNullException_WhenHandlerIsNull()
        {
            var task = CreateResponseTask(HttpStatusCode.OK);

            await Should.ThrowAsync<ArgumentNullException>(async () =>
            {
                await task.When(_ => true, (Func<HttpResponseMessage, Task>)null!);
            });
        }

        [Fact]
        public async Task PropagatesException_ThrownByAsyncHandler()
        {
            var task = CreateResponseTask(HttpStatusCode.OK);

            await Should.ThrowAsync<InvalidOperationException>(async () =>
            {
                await task.When(_ => true, _ => throw new InvalidOperationException("boom"));
            });
        }
    }

    public class OnSuccess_SyncHandler
    {
        [Fact]
        public async Task InvokesHandler_WhenResponseIsSuccessful()
        {
            var called = false;

            var responseTask = CreateResponseTask(HttpStatusCode.OK)
                .OnSuccess(_ => called = true);

            var response = await responseTask;

            called.ShouldBeTrue();
            response.IsSuccessStatusCode.ShouldBeTrue();
        }

        [Fact]
        public async Task DoesNotInvokeHandler_WhenResponseIsNotSuccessful()
        {
            var called = false;

            var responseTask = CreateResponseTask(HttpStatusCode.BadRequest)
                .OnSuccess(_ => called = true);

            var response = await responseTask;

            called.ShouldBeFalse();
            response.IsSuccessStatusCode.ShouldBeFalse();
        }

        [Fact]
        public async Task ThrowsArgumentNullException_WhenHandlerIsNull()
        {
            var task = CreateResponseTask(HttpStatusCode.OK);

            await Should.ThrowAsync<ArgumentNullException>(async () =>
            {
                await task.OnSuccess((Action<HttpResponseMessage>)null!);
            });
        }
    }

    public class OnSuccess_AsyncHandler
    {
        [Fact]
        public async Task InvokesAsyncHandler_WhenResponseIsSuccessful()
        {
            var called = false;

            var responseTask = CreateResponseTask(HttpStatusCode.OK)
                .OnSuccess(async _ =>
                {
                    await Task.Yield();
                    called = true;
                });

            var response = await responseTask;

            called.ShouldBeTrue();
            response.IsSuccessStatusCode.ShouldBeTrue();
        }

        [Fact]
        public async Task DoesNotInvokeAsyncHandler_WhenResponseIsNotSuccessful()
        {
            var called = false;

            var responseTask = CreateResponseTask(HttpStatusCode.BadRequest)
                .OnSuccess(async _ =>
                {
                    await Task.Yield();
                    called = true;
                });

            var response = await responseTask;

            called.ShouldBeFalse();
            response.IsSuccessStatusCode.ShouldBeFalse();
        }

        [Fact]
        public async Task ThrowsArgumentNullException_WhenHandlerIsNull()
        {
            var task = CreateResponseTask(HttpStatusCode.OK);

            await Should.ThrowAsync<ArgumentNullException>(async () =>
            {
                await task.OnSuccess((Func<HttpResponseMessage, Task>)null!);
            });
        }
    }

    public class OnFailure_SyncHandler
    {
        [Fact]
        public async Task InvokesHandler_WhenResponseIsNotSuccessful()
        {
            var called = false;

            var responseTask = CreateResponseTask(HttpStatusCode.BadRequest)
                .OnFailure(_ => called = true);

            var response = await responseTask;

            called.ShouldBeTrue();
            response.IsSuccessStatusCode.ShouldBeFalse();
        }

        [Fact]
        public async Task DoesNotInvokeHandler_WhenResponseIsSuccessful()
        {
            var called = false;

            var responseTask = CreateResponseTask(HttpStatusCode.OK)
                .OnFailure(_ => called = true);

            var response = await responseTask;

            called.ShouldBeFalse();
            response.IsSuccessStatusCode.ShouldBeTrue();
        }

        [Fact]
        public async Task ThrowsArgumentNullException_WhenHandlerIsNull()
        {
            var task = CreateResponseTask(HttpStatusCode.BadRequest);

            await Should.ThrowAsync<ArgumentNullException>(async () =>
            {
                await task.OnFailure((Action<HttpResponseMessage>)null!);
            });
        }
    }

    public class OnFailure_AsyncHandler
    {
        [Fact]
        public async Task InvokesAsyncHandler_WhenResponseIsNotSuccessful()
        {
            var called = false;

            var responseTask = CreateResponseTask(HttpStatusCode.BadRequest)
                .OnFailure(async _ =>
                {
                    await Task.Yield();
                    called = true;
                });

            var response = await responseTask;

            called.ShouldBeTrue();
            response.IsSuccessStatusCode.ShouldBeFalse();
        }

        [Fact]
        public async Task DoesNotInvokeAsyncHandler_WhenResponseIsSuccessful()
        {
            var called = false;

            var responseTask = CreateResponseTask(HttpStatusCode.OK)
                .OnFailure(async _ =>
                {
                    await Task.Yield();
                    called = true;
                });

            var response = await responseTask;

            called.ShouldBeFalse();
            response.IsSuccessStatusCode.ShouldBeTrue();
        }

        [Fact]
        public async Task ThrowsArgumentNullException_WhenHandlerIsNull()
        {
            var task = CreateResponseTask(HttpStatusCode.BadRequest);

            await Should.ThrowAsync<ArgumentNullException>(async () =>
            {
                await task.OnFailure((Func<HttpResponseMessage, Task>)null!);
            });
        }
    }
}
