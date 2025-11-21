#if NET7_0_OR_GREATER
using System.Net;
using System.Text;
using System.Text.Json;

namespace FluentHttpClient.Tests;

public class FluentJsonTypedDeserializationAotTests
{
    public class HttpResponseMessageExtensions
    {
        [Fact]
        public async Task ReadJsonAsync_DeserializesResponseContent_WhenUsingJsonTypeInfo()
        {
            var expected = new SampleModel { Id = 42, Name = "Grant" };
            var json = JsonSerializer.Serialize(expected);

            using var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var result = await response.ReadJsonAsync(SampleModelJsonContext.Default.SampleModel);

            result.ShouldNotBeNull();
            result!.Id.ShouldBe(expected.Id);
            result.Name.ShouldBe(expected.Name);
        }

        [Fact]
        public async Task ReadJsonAsync_ReturnsDefault_WhenResponseContentIsNull()
        {
            using var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = null!
            };

            var result = await response.ReadJsonAsync(SampleModelJsonContext.Default.SampleModel);

            result.ShouldBeNull();
        }

        [Fact]
        public async Task ReadJsonAsync_DeserializesResponseContent_WhenUsingJsonSerializerContext()
        {
            var expected = new SampleModel { Id = 7, Name = "Lex" };
            var json = JsonSerializer.Serialize(expected);

            using var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var result = await response.ReadJsonAsync<SampleModel>(
                SampleModelJsonContext.Default,
                CancellationToken.None);

            result.ShouldNotBeNull();
            result!.Id.ShouldBe(expected.Id);
            result.Name.ShouldBe(expected.Name);
        }

        [Fact]
        public async Task ReadJsonAsync_ThrowsInvalidOperation_WhenContextMissingTypeInfo()
        {
            var expected = new SampleModel { Id = 99, Name = "Ellie" };
            var json = JsonSerializer.Serialize(expected);

            using var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var exception = await Should.ThrowAsync<InvalidOperationException>(
                async () => await response.ReadJsonAsync<SampleModel>(
                    OtherModelJsonContext.Default,
                    CancellationToken.None));

            exception.Message.ShouldContain(nameof(SampleModel));
        }
    }

    public class TaskOfHttpResponseMessageExtensions
    {
        [Fact]
        public async Task ReadJsonAsync_DeserializesResponseTask_WhenUsingJsonTypeInfo()
        {
            var expected = new SampleModel { Id = 13, Name = "Malcolm" };
            var json = JsonSerializer.Serialize(expected);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var responseTask = Task.FromResult(response);

            var result = await responseTask.ReadJsonAsync(
                SampleModelJsonContext.Default.SampleModel,
                CancellationToken.None);

            result.ShouldNotBeNull();
            result!.Id.ShouldBe(expected.Id);
            result.Name.ShouldBe(expected.Name);

            response.Dispose();
        }

        [Fact]
        public async Task ReadJsonAsync_DeserializesResponseTask_WhenUsingJsonSerializerContext()
        {
            var expected = new SampleModel { Id = 21, Name = "Sattler" };
            var json = JsonSerializer.Serialize(expected);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var responseTask = Task.FromResult(response);

            var result = await responseTask.ReadJsonAsync<SampleModel>(
                SampleModelJsonContext.Default,
                CancellationToken.None);

            result.ShouldNotBeNull();
            result!.Id.ShouldBe(expected.Id);
            result.Name.ShouldBe(expected.Name);

            response.Dispose();
        }
    }
}

#endif
