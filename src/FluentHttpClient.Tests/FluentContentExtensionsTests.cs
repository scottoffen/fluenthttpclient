using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Xml;

namespace FluentHttpClient.Tests;

public class FluentContentExtensionsTests
{
    internal static HttpRequestBuilder CreateBuilder()
        => new HttpRequestBuilder(new HttpClient(), "https://example.com/");

    public class BufferedContentTests
    {
        [Fact]
        public void WithBufferedContent_SetsBufferRequestContentToTrue_WhenCalled()
        {
            var builder = new HttpRequestBuilder(new HttpClient(), "https://example.com");

            builder.BufferRequestContent.ShouldBeFalse();

            var result = builder.WithBufferedContent();

            builder.BufferRequestContent.ShouldBeTrue();
            result.ShouldBeSameAs(builder);
        }
    }

    public class HttpContentTests
    {
        [Fact]
        public void WithContent_UsesProvidedHttpContentInstance_WhenCalled()
        {
            var builder = CreateBuilder();
            var content = new StringContent("payload");

            builder.WithContent(content);

            builder.Content.ShouldBeSameAs(content);
        }
    }

    public class FormContentTests
    {
        [Fact]
        public async Task WithFormContent_UsesDictionaryValues_WhenUsingDictionaryOverload()
        {
            var builder = CreateBuilder();
            var data = new Dictionary<string, string>
            {
                ["foo"] = "bar",
                ["baz"] = "qux"
            };

            builder.WithFormContent(data);

            builder.Content.ShouldBeOfType<FormUrlEncodedContent>();
            builder.Content.ShouldNotBeNull();
            var encoded = await builder.Content!.ReadAsStringAsync();
            encoded.ShouldContain("foo=bar");
            encoded.ShouldContain("baz=qux");
        }

        [Fact]
        public async Task WithFormContent_AllowsDuplicateKeys_WhenUsingEnumerableOverload()
        {
            var builder = CreateBuilder();
            var data = new List<KeyValuePair<string, string>>
            {
                new("key", "value1"),
                new("key", "value2")
            };

            builder.WithFormContent(data);

            builder.Content.ShouldBeOfType<FormUrlEncodedContent>();
            builder.Content.ShouldNotBeNull();
            var encoded = await builder.Content!.ReadAsStringAsync();
            encoded.ShouldContain("key=value1");
            encoded.ShouldContain("key=value2");
        }
    }

    public class StringContentTests
    {
        [Fact]
        public async Task WithContent_SetsStringContentWithProvidedValue_WhenUsingStringOverload()
        {
            var builder = CreateBuilder();

            builder.WithContent("hello world");

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            var body = await builder.Content!.ReadAsStringAsync();
            body.ShouldBe("hello world");
        }

        [Fact]
        public void WithContent_UsesProvidedEncoding_WhenEncodingIsSpecified()
        {
            var builder = CreateBuilder();
            var encoding = Encoding.Unicode;

            builder.WithContent("value", encoding);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            builder.Content!.Headers.ContentType.ShouldNotBeNull();
            builder.Content.Headers.ContentType!.CharSet.ShouldBe(encoding.WebName);
        }

        [Fact]
        public void WithContent_UsesUtf8AndSpecifiedMediaType_WhenMediaTypeStringOverloadUsed()
        {
            var builder = CreateBuilder();
            var mediaType = "text/plain";

            builder.WithContent("value", mediaType);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            builder.Content!.Headers.ContentType.ShouldNotBeNull();
            builder.Content.Headers.ContentType!.MediaType.ShouldBe(mediaType);
            builder.Content.Headers.ContentType!.CharSet.ShouldBe(Encoding.UTF8.WebName);
        }

        [Fact]
        public void WithContent_UsesProvidedEncodingAndMediaType_WhenEncodingAndMediaTypeStringOverloadUsed()
        {
            var builder = CreateBuilder();
            var encoding = Encoding.Unicode;
            var mediaType = "application/custom";

            builder.WithContent("value", encoding, mediaType);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            builder.Content!.Headers.ContentType.ShouldNotBeNull();
            builder.Content.Headers.ContentType!.MediaType.ShouldBe(mediaType);
            builder.Content.Headers.ContentType!.CharSet.ShouldBe(encoding.WebName);
        }

        [Fact]
        public void WithContent_UsesUtf8AndMediaTypeHeader_WhenMediaTypeHeaderIsSpecified()
        {
            var builder = CreateBuilder();
            var header = new MediaTypeHeaderValue("application/custom");

            builder.WithContent("value", header);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            builder.Content!.Headers.ContentType.ShouldBeSameAs(header);
        }

        [Fact]
        public void WithContent_UsesEncodingAndMediaTypeHeader_WhenEncodingAndHeaderSpecified()
        {
            var builder = CreateBuilder();
            var encoding = Encoding.Unicode;
            var header = new MediaTypeHeaderValue("application/custom");

            builder.WithContent("value", encoding, header);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            builder.Content!.Headers.ContentType.ShouldBeSameAs(header);
        }
    }

    public class XmlContentTests
    {
        public sealed class SampleDto
        {
            public string? Name { get; set; }
            public int Value { get; set; }
        }

        [Fact]
        public async Task WithXmlContent_UsesUtf8AndDefaultXmlContentType_WhenXmlStringOverloadIsUsed()
        {
            var builder = CreateBuilder();
            var xml = "<root>value</root>";

            builder.WithXmlContent(xml);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            var body = await builder.Content!.ReadAsStringAsync();
            body.ShouldBe(xml);
            builder.Content.Headers.ContentType.ShouldNotBeNull();
            builder.Content.Headers.ContentType!.MediaType.ShouldBe(FluentXmlSerializer.DefaultContentType);
            builder.Content.Headers.ContentType!.CharSet.ShouldBe(Encoding.UTF8.WebName);
        }

        [Fact]
        public void WithXmlContent_UsesProvidedEncoding_WhenEncodingSpecifiedForXmlString()
        {
            var builder = CreateBuilder();
            var xml = "<root>value</root>";
            var encoding = Encoding.Unicode;

            builder.WithXmlContent(xml, encoding);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            builder.Content!.Headers.ContentType.ShouldNotBeNull();
            builder.Content.Headers.ContentType!.MediaType.ShouldBe(FluentXmlSerializer.DefaultContentType);
            builder.Content.Headers.ContentType!.CharSet.ShouldBe(encoding.WebName);
        }

        [Fact]
        public void WithXmlContent_UsesUtf8AndSpecifiedContentType_WhenContentTypeStringProvided()
        {
            var builder = CreateBuilder();
            var xml = "<root>value</root>";
            var contentType = "application/custom-xml";

            builder.WithXmlContent(xml, contentType);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            builder.Content!.Headers.ContentType.ShouldNotBeNull();
            builder.Content.Headers.ContentType!.MediaType.ShouldBe(contentType);
            builder.Content.Headers.ContentType!.CharSet.ShouldBe(Encoding.UTF8.WebName);
        }

        [Fact]
        public void WithXmlContent_UsesEncodingAndSpecifiedContentType_WhenEncodingAndContentTypeStringProvided()
        {
            var builder = CreateBuilder();
            var xml = "<root>value</root>";
            var encoding = Encoding.Unicode;
            var contentType = "application/custom-xml";

            builder.WithXmlContent(xml, encoding, contentType);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            builder.Content!.Headers.ContentType.ShouldNotBeNull();
            builder.Content.Headers.ContentType!.MediaType.ShouldBe(contentType);
            builder.Content.Headers.ContentType!.CharSet.ShouldBe(encoding.WebName);
        }

        [Fact]
        public void WithXmlContent_UsesMediaTypeHeader_WhenMediaTypeHeaderIsSpecified()
        {
            var builder = CreateBuilder();
            var xml = "<root>value</root>";
            var header = new MediaTypeHeaderValue("application/custom-xml");

            builder.WithXmlContent(xml, header);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            builder.Content!.Headers.ContentType.ShouldBeSameAs(header);
        }

        [Fact]
        public void WithXmlContent_UsesEncodingAndMediaTypeHeader_WhenEncodingAndHeaderSpecified()
        {
            var builder = CreateBuilder();
            var xml = "<root>value</root>";
            var encoding = Encoding.Unicode;
            var header = new MediaTypeHeaderValue("application/custom-xml");

            builder.WithXmlContent(xml, encoding, header);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            builder.Content!.Headers.ContentType.ShouldBeSameAs(header);
        }

        [Fact]
        public async Task WithXmlContent_UsesFluentXmlSerializer_WhenSerializingObject()
        {
            var builder = CreateBuilder();
            var dto = new SampleDto { Name = "test", Value = 42 };

            builder.WithXmlContent(dto);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            var body = await builder.Content!.ReadAsStringAsync();
            var expected = FluentXmlSerializer.Serialize<SampleDto>(dto);
            body.ShouldBe(expected);
            builder.Content.Headers.ContentType.ShouldNotBeNull();
            builder.Content.Headers.ContentType!.MediaType.ShouldBe(FluentXmlSerializer.DefaultContentType);
        }

        [Fact]
        public void WithXmlContent_UsesXmlWriterSettingsEncoding_WhenSettingsProvided()
        {
            var builder = CreateBuilder();
            var dto = new SampleDto { Name = "test", Value = 42 };
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.Unicode
            };

            builder.WithXmlContent(dto, settings);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            builder.Content!.Headers.ContentType.ShouldNotBeNull();
            builder.Content.Headers.ContentType!.CharSet.ShouldBe(settings.Encoding!.WebName);
        }

        [Fact]
        public async Task WithXmlContent_UsesContentTypeString_WhenSerializingObjectWithContentType()
        {
            var builder = CreateBuilder();
            var dto = new SampleDto { Name = "test", Value = 42 };
            var contentType = "application/custom-xml";

            builder.WithXmlContent(dto, contentType);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            var body = await builder.Content!.ReadAsStringAsync();
            var expected = FluentXmlSerializer.Serialize<SampleDto>(dto);
            body.ShouldBe(expected);
            builder.Content.Headers.ContentType.ShouldNotBeNull();
            builder.Content.Headers.ContentType!.MediaType.ShouldBe(contentType);
        }

        [Fact]
        public void WithXmlContent_UsesSettingsEncodingAndContentTypeString_WhenSettingsAndContentTypeProvided()
        {
            var builder = CreateBuilder();
            var dto = new SampleDto { Name = "test", Value = 42 };
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.Unicode
            };
            var contentType = "application/custom-xml";

            builder.WithXmlContent(dto, settings, contentType);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            builder.Content!.Headers.ContentType.ShouldNotBeNull();
            builder.Content.Headers.ContentType!.MediaType.ShouldBe(contentType);
            builder.Content.Headers.ContentType!.CharSet.ShouldBe(settings.Encoding!.WebName);
        }

        [Fact]
        public void WithXmlContent_UsesMediaTypeHeader_WhenSerializingObjectWithHeader()
        {
            var builder = CreateBuilder();
            var dto = new SampleDto { Name = "test", Value = 42 };
            var header = new MediaTypeHeaderValue("application/custom-xml");

            builder.WithXmlContent(dto, header);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            builder.Content!.Headers.ContentType.ShouldBeSameAs(header);
        }

        [Fact]
        public void WithXmlContent_UsesSettingsEncodingAndMediaTypeHeader_WhenSettingsAndHeaderProvided()
        {
            var builder = CreateBuilder();
            var dto = new SampleDto { Name = "test", Value = 42 };
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.Unicode
            };
            var header = new MediaTypeHeaderValue("application/custom-xml");

            builder.WithXmlContent(dto, settings, header);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            builder.Content!.Headers.ContentType.ShouldBeSameAs(header);
        }
    }

    public class JsonContentTests
    {
        private sealed class SampleDto
        {
            public string? Name { get; set; }
            public int Value { get; set; }
        }

        [Fact]
        public async Task WithJsonContent_UsesUtf8AndDefaultJsonContentType_WhenJsonStringOverloadIsUsed()
        {
            var builder = CreateBuilder();
            var json = """{"name":"test","value":42}""";

            builder.WithJsonContent(json);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            var body = await builder.Content!.ReadAsStringAsync();
            body.ShouldBe(json);
            builder.Content.Headers.ContentType.ShouldNotBeNull();
            builder.Content.Headers.ContentType!.MediaType.ShouldBe(FluentJsonSerializer.DefaultContentType);
            builder.Content.Headers.ContentType!.CharSet.ShouldBe(Encoding.UTF8.WebName);
        }

        [Fact]
        public void WithJsonContent_UsesProvidedEncoding_WhenEncodingSpecifiedForJsonString()
        {
            var builder = CreateBuilder();
            var json = """{"name":"test"}""";
            var encoding = Encoding.Unicode;

            builder.WithJsonContent(json, encoding);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            builder.Content!.Headers.ContentType.ShouldNotBeNull();
            builder.Content.Headers.ContentType!.MediaType.ShouldBe(FluentJsonSerializer.DefaultContentType);
            builder.Content.Headers.ContentType!.CharSet.ShouldBe(encoding.WebName);
        }

        [Fact]
        public void WithJsonContent_UsesUtf8AndSpecifiedContentType_WhenContentTypeStringProvided()
        {
            var builder = CreateBuilder();
            var json = """{"name":"test"}""";
            var contentType = "application/vnd.custom+json";

            builder.WithJsonContent(json, contentType);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            builder.Content!.Headers.ContentType.ShouldNotBeNull();
            builder.Content.Headers.ContentType!.MediaType.ShouldBe(contentType);
            builder.Content.Headers.ContentType!.CharSet.ShouldBe(Encoding.UTF8.WebName);
        }

        [Fact]
        public void WithJsonContent_UsesEncodingAndSpecifiedContentType_WhenEncodingAndContentTypeStringProvided()
        {
            var builder = CreateBuilder();
            var json = """{"name":"test"}""";
            var encoding = Encoding.Unicode;
            var contentType = "application/vnd.custom+json";

            builder.WithJsonContent(json, encoding, contentType);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            builder.Content!.Headers.ContentType.ShouldNotBeNull();
            builder.Content.Headers.ContentType!.MediaType.ShouldBe(contentType);
            builder.Content.Headers.ContentType!.CharSet.ShouldBe(encoding.WebName);
        }

        [Fact]
        public void WithJsonContent_UsesMediaTypeHeader_WhenMediaTypeHeaderIsSpecified()
        {
            var builder = CreateBuilder();
            var json = """{"name":"test"}""";
            var header = new MediaTypeHeaderValue("application/vnd.custom+json");

            builder.WithJsonContent(json, header);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            builder.Content!.Headers.ContentType.ShouldBeSameAs(header);
        }

        [Fact]
        public void WithJsonContent_UsesEncodingAndMediaTypeHeader_WhenEncodingAndHeaderSpecified()
        {
            var builder = CreateBuilder();
            var json = """{"name":"test"}""";
            var encoding = Encoding.Unicode;
            var header = new MediaTypeHeaderValue("application/vnd.custom+json");

            builder.WithJsonContent(json, encoding, header);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            builder.Content!.Headers.ContentType.ShouldBeSameAs(header);
        }

        [Fact]
        public async Task WithJsonContent_UsesDefaultSerializerOptions_WhenSerializingObject()
        {
            var builder = CreateBuilder();
            var dto = new SampleDto { Name = "test", Value = 42 };

            builder.WithJsonContent(dto);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            var body = await builder.Content!.ReadAsStringAsync();
            var expected = JsonSerializer.Serialize(dto, FluentJsonSerializer.DefaultJsonSerializerOptions);
            body.ShouldBe(expected);
            builder.Content.Headers.ContentType.ShouldNotBeNull();
            builder.Content.Headers.ContentType!.MediaType.ShouldBe(FluentJsonSerializer.DefaultContentType);
        }

        [Fact]
        public async Task WithJsonContent_UsesProvidedSerializerOptions_WhenCustomOptionsProvided()
        {
            var builder = CreateBuilder();
            var dto = new SampleDto { Name = "TestName", Value = 42 };
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            builder.WithJsonContent(dto, options);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            var body = await builder.Content!.ReadAsStringAsync();
            var expected = JsonSerializer.Serialize(dto, options);
            body.ShouldBe(expected);
        }

        [Fact]
        public async Task WithJsonContent_UsesContentTypeString_WhenSerializingObjectWithContentType()
        {
            var builder = CreateBuilder();
            var dto = new SampleDto { Name = "test", Value = 42 };
            var contentType = "application/vnd.custom+json";

            builder.WithJsonContent(dto, contentType);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            var body = await builder.Content!.ReadAsStringAsync();
            var expected = JsonSerializer.Serialize(dto, FluentJsonSerializer.DefaultJsonSerializerOptions);
            body.ShouldBe(expected);
            builder.Content.Headers.ContentType.ShouldNotBeNull();
            builder.Content.Headers.ContentType!.MediaType.ShouldBe(contentType);
        }

        [Fact]
        public async Task WithJsonContent_UsesOptionsAndContentTypeString_WhenOptionsAndContentTypeProvided()
        {
            var builder = CreateBuilder();
            var dto = new SampleDto { Name = "test", Value = 42 };
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var contentType = "application/vnd.custom+json";

            builder.WithJsonContent(dto, options, contentType);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            var body = await builder.Content!.ReadAsStringAsync();
            var expected = JsonSerializer.Serialize(dto, options);
            body.ShouldBe(expected);
            builder.Content.Headers.ContentType.ShouldNotBeNull();
            builder.Content.Headers.ContentType!.MediaType.ShouldBe(contentType);
        }

        [Fact]
        public void WithJsonContent_UsesMediaTypeHeader_WhenSerializingObjectWithHeader()
        {
            var builder = CreateBuilder();
            var dto = new SampleDto { Name = "test", Value = 42 };
            var header = new MediaTypeHeaderValue("application/vnd.custom+json");

            builder.WithJsonContent(dto, header);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            builder.Content!.Headers.ContentType.ShouldBeSameAs(header);
        }

        [Fact]
        public void WithJsonContent_UsesOptionsAndMediaTypeHeader_WhenOptionsAndHeaderProvided()
        {
            var builder = CreateBuilder();
            var dto = new SampleDto { Name = "test", Value = 42 };
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var header = new MediaTypeHeaderValue("application/vnd.custom+json");

            builder.WithJsonContent(dto, options, header);

            builder.Content.ShouldBeOfType<StringContent>();
            builder.Content.ShouldNotBeNull();
            builder.Content!.Headers.ContentType.ShouldBeSameAs(header);
        }
    }
}
