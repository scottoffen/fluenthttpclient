#if NET7_0_OR_GREATER
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentHttpClient;
using Shouldly;
using Xunit;

namespace FluentHttpClient.Tests;

public class FluentJsonContentExtensionsAotTests
{
    private static HttpRequestBuilder CreateBuilder()
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri("https://example.com/")
        };

        return client.UsingBase();
    }

    public class JsonTypeInfoOverloads
    {
        [Fact]
        public async Task WithJsonContent_SetsUtf8JsonContentWithDefaultMediaType_WhenUsingJsonTypeInfo()
        {
            var builder = CreateBuilder();
            var model = new SampleModel { Id = 42, Name = "Grant" };

            builder.WithJsonContent(model, SampleModelJsonContext.Default.SampleModel);

            builder.Content.ShouldNotBeNull();

            var content = builder.Content as StringContent;
            content.ShouldNotBeNull();

            var payload = await content!.ReadAsStringAsync();
            payload.ShouldBe(JsonSerializer.Serialize(model, SampleModelJsonContext.Default.SampleModel));

            content.Headers.ContentType.ShouldNotBeNull();
            content.Headers.ContentType!.MediaType.ShouldBe(FluentJsonSerializer.DefaultContentType);
            content.Headers.ContentType!.CharSet.ShouldBe(Encoding.UTF8.WebName);
        }

        [Fact]
        public async Task WithJsonContent_SetsUtf8JsonContentWithSpecifiedMediaType_WhenUsingJsonTypeInfo()
        {
            var builder = CreateBuilder();
            var model = new SampleModel { Id = 7, Name = "Lex" };
            const string contentType = "application/vnd.sample+json";

            builder.WithJsonContent(model, SampleModelJsonContext.Default.SampleModel, contentType);

            var content = builder.Content as StringContent;
            content.ShouldNotBeNull();

            var payload = await content!.ReadAsStringAsync();
            payload.ShouldBe(JsonSerializer.Serialize(model, SampleModelJsonContext.Default.SampleModel));

            content.Headers.ContentType.ShouldNotBeNull();
            content.Headers.ContentType!.MediaType.ShouldBe(contentType);
            content.Headers.ContentType!.CharSet.ShouldBe(Encoding.UTF8.WebName);
        }

        [Fact]
        public async Task WithJsonContent_SetsUtf8JsonContentWithSpecifiedMediaTypeHeader_WhenUsingJsonTypeInfo()
        {
            var builder = CreateBuilder();
            var model = new SampleModel { Id = 13, Name = "Malcolm" };
            var header = new MediaTypeHeaderValue("application/json")
            {
                CharSet = "utf-8"
            };

            builder.WithJsonContent(model, SampleModelJsonContext.Default.SampleModel, header);

            var content = builder.Content as StringContent;
            content.ShouldNotBeNull();

            var payload = await content!.ReadAsStringAsync();
            payload.ShouldBe(JsonSerializer.Serialize(model, SampleModelJsonContext.Default.SampleModel));

            content.Headers.ContentType.ShouldNotBeNull();
            content.Headers.ContentType!.MediaType.ShouldBe(header.MediaType);
            content.Headers.ContentType!.CharSet.ShouldBe(header.CharSet);
        }
    }

    public class JsonSerializerContextOverloads
    {
        [Fact]
        public async Task WithJsonContent_SetsUtf8JsonContentWithDefaultMediaType_WhenUsingJsonSerializerContext()
        {
            var builder = CreateBuilder();
            var model = new SampleModel { Id = 21, Name = "Sattler" };

            builder.WithJsonContent(model, SampleModelJsonContext.Default);

            var content = builder.Content as StringContent;
            content.ShouldNotBeNull();

            var payload = await content!.ReadAsStringAsync();
            payload.ShouldBe(JsonSerializer.Serialize(model, SampleModelJsonContext.Default.SampleModel));

            content.Headers.ContentType.ShouldNotBeNull();
            content.Headers.ContentType!.MediaType.ShouldBe(FluentJsonSerializer.DefaultContentType);
            content.Headers.ContentType!.CharSet.ShouldBe(Encoding.UTF8.WebName);
        }

        [Fact]
        public async Task WithJsonContent_SetsUtf8JsonContentWithSpecifiedMediaType_WhenUsingJsonSerializerContext()
        {
            var builder = CreateBuilder();
            var model = new SampleModel { Id = 5, Name = "Tim" };
            const string contentType = "application/problem+json";

            builder.WithJsonContent(model, SampleModelJsonContext.Default, contentType);

            var content = builder.Content as StringContent;
            content.ShouldNotBeNull();

            var payload = await content!.ReadAsStringAsync();
            payload.ShouldBe(JsonSerializer.Serialize(model, SampleModelJsonContext.Default.SampleModel));

            content.Headers.ContentType.ShouldNotBeNull();
            content.Headers.ContentType!.MediaType.ShouldBe(contentType);
            content.Headers.ContentType!.CharSet.ShouldBe(Encoding.UTF8.WebName);
        }

        [Fact]
        public async Task WithJsonContent_SetsUtf8JsonContentWithSpecifiedMediaTypeHeader_WhenUsingJsonSerializerContext()
        {
            var builder = CreateBuilder();
            var model = new SampleModel { Id = 8, Name = "Ellie" };
            var header = new MediaTypeHeaderValue("application/json")
            {
                CharSet = "utf-8"
            };

            builder.WithJsonContent(model, SampleModelJsonContext.Default, header);

            var content = builder.Content as StringContent;
            content.ShouldNotBeNull();

            var payload = await content!.ReadAsStringAsync();
            payload.ShouldBe(JsonSerializer.Serialize(model, SampleModelJsonContext.Default.SampleModel));

            content.Headers.ContentType.ShouldNotBeNull();
            content.Headers.ContentType!.MediaType.ShouldBe(header.MediaType);
            content.Headers.ContentType!.CharSet.ShouldBe(header.CharSet);
        }

        [Fact]
        public void WithJsonContent_ThrowsInvalidOperation_WhenContextMissingTypeInfoForT()
        {
            var builder = CreateBuilder();
            var model = new SampleModel { Id = 99, Name = "Dodgson" };

            var exception = Should.Throw<InvalidOperationException>(
                () => builder.WithJsonContent(model, OtherModelJsonContext.Default));

            exception.Message.ShouldContain(nameof(SampleModel));
        }
    }
}

#endif
