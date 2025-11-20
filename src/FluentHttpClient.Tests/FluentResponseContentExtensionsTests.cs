using System.Text;

namespace FluentHttpClient.Tests;

public class FluentResponseContentExtensionsTests
{
    public class StringContentTests
    {
        [Fact]
        public async Task ReadContentAsStringAsync_ReturnsBody_WhenContentPresent()
        {
            using var response = new HttpResponseMessage
            {
                Content = new StringContent("hello world", Encoding.UTF8)
            };

            var result = await response.ReadContentAsStringAsync();

            result.ShouldBe("hello world");
        }

        [Fact]
        public async Task ReadContentAsStringAsync_ReturnsEmptyString_WhenContentIsExplicitlyNull()
        {
            using var response = new HttpResponseMessage
            {
                Content = null
            };

            var result = await response.ReadContentAsStringAsync();

            result.ShouldBe(string.Empty);
        }

        [Fact]
        public async Task ReadContentAsStringAsync_ReturnsEmptyString_WhenContentIsEmpty()
        {
            using var response = new HttpResponseMessage
            {
                Content = new StringContent(string.Empty, Encoding.UTF8)
            };

            var result = await response.ReadContentAsStringAsync();

            result.ShouldBe(string.Empty);
        }

        [Fact]
        public async Task ReadContentAsStringAsync_TaskOverload_ReturnsBody_WhenContentPresent()
        {
            using var response = new HttpResponseMessage
            {
                Content = new StringContent("from task", Encoding.UTF8)
            };

            Task<HttpResponseMessage> responseTask = Task.FromResult(response);

            var result = await responseTask.ReadContentAsStringAsync();

            result.ShouldBe("from task");
        }

        [Fact]
        public async Task ReadContentAsStringAsync_TaskOverload_ReturnsEmptyString_WhenContentIsExplicitlyNull()
        {
            using var response = new HttpResponseMessage
            {
                Content = null
            };

            Task<HttpResponseMessage> responseTask = Task.FromResult(response);

            var result = await responseTask.ReadContentAsStringAsync();

            result.ShouldBe(string.Empty);
        }

        [Fact]
        public async Task ReadContentAsStringAsync_TaskOverload_ReturnsEmptyString_WhenContentIsEmpty()
        {
            using var response = new HttpResponseMessage
            {
                Content = new StringContent(string.Empty, Encoding.UTF8)
            };

            Task<HttpResponseMessage> responseTask = Task.FromResult(response);

            var result = await responseTask.ReadContentAsStringAsync();

            result.ShouldBe(string.Empty);
        }
    }

    public class StreamContentTests
    {
        [Fact]
        public async Task ReadContentAsStreamAsync_ReturnsReadableStream_WhenContentPresent()
        {
            var text = "stream content";

            using var response = new HttpResponseMessage
            {
                Content = new StringContent(text, Encoding.UTF8)
            };

            var stream = await response.ReadContentAsStreamAsync();

            stream.ShouldNotBeNull();
            stream.CanRead.ShouldBeTrue();

            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            var result = await reader.ReadToEndAsync();
            result.ShouldBe(text);
        }

        [Fact]
        public async Task ReadContentAsStreamAsync_ReturnsEmptyReadableStream_WhenContentIsExplicitlyNull()
        {
            using var response = new HttpResponseMessage
            {
                Content = null
            };

            var stream = await response.ReadContentAsStreamAsync();

            stream.ShouldNotBeNull();
            stream.CanRead.ShouldBeTrue();

            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            var result = await reader.ReadToEndAsync();
            result.ShouldBe(string.Empty);
        }

        [Fact]
        public async Task ReadContentAsStreamAsync_ReturnsEmptyReadableStream_WhenContentIsEmpty()
        {
            using var response = new HttpResponseMessage
            {
                Content = new StringContent(string.Empty, Encoding.UTF8)
            };

            var stream = await response.ReadContentAsStreamAsync();

            stream.ShouldNotBeNull();
            stream.CanRead.ShouldBeTrue();

            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            var result = await reader.ReadToEndAsync();
            result.ShouldBe(string.Empty);
        }

        [Fact]
        public async Task ReadContentAsStreamAsync_TaskOverload_ReturnsReadableStream_WhenContentPresent()
        {
            var text = "stream from task";

            using var response = new HttpResponseMessage
            {
                Content = new StringContent(text, Encoding.UTF8)
            };

            Task<HttpResponseMessage> responseTask = Task.FromResult(response);

            var stream = await responseTask.ReadContentAsStreamAsync();

            stream.ShouldNotBeNull();
            stream.CanRead.ShouldBeTrue();

            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            var result = await reader.ReadToEndAsync();
            result.ShouldBe(text);
        }

        [Fact]
        public async Task ReadContentAsStreamAsync_TaskOverload_ReturnsEmptyReadableStream_WhenContentIsExplicitlyNull()
        {
            using var response = new HttpResponseMessage
            {
                Content = null
            };

            Task<HttpResponseMessage> responseTask = Task.FromResult(response);

            var stream = await responseTask.ReadContentAsStreamAsync();

            stream.ShouldNotBeNull();
            stream.CanRead.ShouldBeTrue();

            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            var result = await reader.ReadToEndAsync();
            result.ShouldBe(string.Empty);
        }

        [Fact]
        public async Task ReadContentAsStreamAsync_TaskOverload_ReturnsEmptyReadableStream_WhenContentIsEmpty()
        {
            using var response = new HttpResponseMessage
            {
                Content = new StringContent(string.Empty, Encoding.UTF8)
            };

            Task<HttpResponseMessage> responseTask = Task.FromResult(response);

            var stream = await responseTask.ReadContentAsStreamAsync();

            stream.ShouldNotBeNull();
            stream.CanRead.ShouldBeTrue();

            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            var result = await reader.ReadToEndAsync();
            result.ShouldBe(string.Empty);
        }
    }

    public class ByteArrayContentTests
    {
        [Fact]
        public async Task ReadContentAsByteArrayAsync_ReturnsBytes_WhenContentPresent()
        {
            var payload = new byte[] { 1, 2, 3, 4 };

            using var response = new HttpResponseMessage
            {
                Content = new ByteArrayContent(payload)
            };

            var result = await response.ReadContentAsByteArrayAsync();

            result.ShouldBe(payload);
        }

        [Fact]
        public async Task ReadContentAsByteArrayAsync_ReturnsEmptyArray_WhenContentIsExplicitlyNull()
        {
            using var response = new HttpResponseMessage
            {
                Content = null
            };

            var result = await response.ReadContentAsByteArrayAsync();

            result.ShouldNotBeNull();
            result.Length.ShouldBe(0);
        }

        [Fact]
        public async Task ReadContentAsByteArrayAsync_ReturnsEmptyArray_WhenContentIsEmpty()
        {
            using var response = new HttpResponseMessage
            {
                Content = new ByteArrayContent([])
            };

            var result = await response.ReadContentAsByteArrayAsync();

            result.ShouldNotBeNull();
            result.Length.ShouldBe(0);
        }

        [Fact]
        public async Task ReadContentAsByteArrayAsync_TaskOverload_ReturnsBytes_WhenContentPresent()
        {
            var payload = new byte[] { 42, 43, 44 };

            using var response = new HttpResponseMessage
            {
                Content = new ByteArrayContent(payload)
            };

            Task<HttpResponseMessage> responseTask = Task.FromResult(response);

            var result = await responseTask.ReadContentAsByteArrayAsync();

            result.ShouldBe(payload);
        }

        [Fact]
        public async Task ReadContentAsByteArrayAsync_TaskOverload_ReturnsEmptyArray_WhenContentIsExplicitlyNull()
        {
            using var response = new HttpResponseMessage
            {
                Content = null
            };

            Task<HttpResponseMessage> responseTask = Task.FromResult(response);

            var result = await responseTask.ReadContentAsByteArrayAsync();

            result.ShouldNotBeNull();
            result.Length.ShouldBe(0);
        }

        [Fact]
        public async Task ReadContentAsByteArrayAsync_TaskOverload_ReturnsEmptyArray_WhenContentIsEmpty()
        {
            using var response = new HttpResponseMessage
            {
                Content = new ByteArrayContent([])
            };

            Task<HttpResponseMessage> responseTask = Task.FromResult(response);

            var result = await responseTask.ReadContentAsByteArrayAsync();

            result.ShouldNotBeNull();
            result.Length.ShouldBe(0);
        }
    }
}
