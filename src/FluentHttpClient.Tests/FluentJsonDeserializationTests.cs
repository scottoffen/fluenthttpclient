using System.Net;
using System.Text;
using System.Text.Json;

namespace FluentHttpClient.Tests;

public class FluentJsonDeserializationTests
{
    private sealed class TestDto
    {
        public string? Name { get; set; }
    }

    private sealed class CaseSensitiveDto
    {
        public string? Value { get; set; }
    }

    public class HttpResponseMessageExtensions
    {
        [Fact]
        public async Task ReadJsonAsync_ReturnsDeserializedInstance_WhenContentIsValidJson()
        {
            var json = "{\"Name\":\"value\"}";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var result = await response.ReadJsonAsync<TestDto>();

            result.ShouldNotBeNull();
            result!.Name.ShouldBe("value");
        }

        [Fact]
        public async Task ReadJsonAsync_UsesProvidedOptions_WhenOptionsAreSpecified()
        {
            var json = "{\"value\":\"test\"}";

            // Case-insensitive options
            var responseInsensitive = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var caseInsensitive = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var insensitiveResult = await responseInsensitive.ReadJsonAsync<CaseSensitiveDto>(caseInsensitive);

            insensitiveResult.ShouldNotBeNull();
            insensitiveResult!.Value.ShouldBe("test");

            // Case-sensitive options (different response to avoid re-reading the same content)
            var responseSensitive = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var caseSensitive = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = false
            };

            var sensitiveResult = await responseSensitive.ReadJsonAsync<CaseSensitiveDto>(caseSensitive);

            sensitiveResult.ShouldNotBeNull();
            sensitiveResult!.Value.ShouldBeNull();
        }

        [Fact]
        public async Task ReadJsonAsync_ThrowsJsonException_WhenContentIsInvalidJson()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{ invalid json", Encoding.UTF8, "application/json")
            };

            await Should.ThrowAsync<JsonException>(async () =>
                await response.ReadJsonAsync<TestDto>());
        }

        [Fact]
        public async Task ReadJsonAsync_ThrowsOperationCanceledException_WhenCancellationRequested()
        {
            var json = "{\"Name\":\"value\"}";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Should.ThrowAsync<OperationCanceledException>(async () =>
                await response.ReadJsonAsync<TestDto>(cts.Token));
        }
    }

    public class HttpResponseMessageTaskExtensions
    {
        [Fact]
        public async Task ReadJsonAsync_DeserializesResult_WhenTaskCompletesSuccessfully()
        {
            var json = "{\"Name\":\"value\"}";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var responseTask = Task.FromResult(response);

            var result = await responseTask.ReadJsonAsync<TestDto>();

            result.ShouldNotBeNull();
            result!.Name.ShouldBe("value");
        }

        [Fact]
        public async Task ReadJsonAsync_ThrowsJsonException_WhenContentIsInvalidJson()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{ invalid json", Encoding.UTF8, "application/json")
            };

            var responseTask = Task.FromResult(response);

            await Should.ThrowAsync<JsonException>(async () =>
                await responseTask.ReadJsonAsync<TestDto>());
        }

        [Fact]
        public async Task ReadJsonAsync_ThrowsOperationCanceledException_WhenCancellationRequested()
        {
            var json = "{\"Name\":\"value\"}";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var responseTask = Task.FromResult(response);

            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Should.ThrowAsync<OperationCanceledException>(async () =>
                await responseTask.ReadJsonAsync<TestDto>(cts.Token));
        }

        [Fact]
        public async Task ReadJsonAsync_UsesProvidedOptions_WhenOptionsAreSpecifiedOnTaskOverload()
        {
            var json = "{\"value\":\"test\"}";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var responseTask = Task.FromResult(response);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = await responseTask.ReadJsonAsync<CaseSensitiveDto>(options);

            result.ShouldNotBeNull();
            result!.Value.ShouldBe("test");
        }
    }
}
