using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace FluentHttpClient.Tests;

public class FluentXmlDeserializationTests
{
    public class SampleModel
    {
        public string? Name { get; set; }
    }

    public class ReadXmlAsyncTypedTests
    {
        [Fact]
        public async Task ReadXmlAsync_ReturnsNull_WhenContentIsNull()
        {
            var response = new HttpResponseMessage
            {
                Content = null
            };

            var result = await response.ReadXmlAsync<SampleModel>();

            result.ShouldBeNull();
        }

        [Fact]
        public async Task ReadXmlAsync_ReturnsNull_WhenContentIsEmpty()
        {
            var response = new HttpResponseMessage
            {
                Content = new StringContent(string.Empty, Encoding.UTF8, "application/xml")
            };

            var result = await response.ReadXmlAsync<SampleModel>();

            result.ShouldBeNull();
        }

        [Fact]
        public async Task ReadXmlAsync_ReturnsNull_WhenContentIsWhitespace()
        {
            var response = new HttpResponseMessage
            {
                Content = new StringContent("   \r\n\t   ", Encoding.UTF8, "application/xml")
            };

            var result = await response.ReadXmlAsync<SampleModel>();

            result.ShouldBeNull();
        }

        [Fact]
        public async Task ReadXmlAsync_ReturnsDeserializedInstance_WhenXmlIsValid()
        {
            var xml = "<SampleModel><Name>Test Name</Name></SampleModel>";
            var response = new HttpResponseMessage
            {
                Content = new StringContent(xml, Encoding.UTF8, "application/xml")
            };

            var result = await response.ReadXmlAsync<SampleModel>();

            result.ShouldNotBeNull();
            result!.Name.ShouldBe("Test Name");
        }

        [Fact]
        public async Task ReadXmlAsync_UsesXmlReaderSettings_WhenProvided()
        {
            var xml = "<SampleModel><Name>Test Name</Name></SampleModel>";
            var response = new HttpResponseMessage
            {
                Content = new StringContent(xml, Encoding.UTF8, "application/xml")
            };

            var settings = new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreWhitespace = true
            };

            var result = await response.ReadXmlAsync<SampleModel>(settings);

            result.ShouldNotBeNull();
            result!.Name.ShouldBe("Test Name");
        }

        [Fact]
        public async Task ReadXmlAsync_ThrowsInvalidOperationException_WhenXmlIsInvalid()
        {
            var xml = "<SampleModel><Name>Test Name</SampleModel>"; // malformed
            var response = new HttpResponseMessage
            {
                Content = new StringContent(xml, Encoding.UTF8, "application/xml")
            };

            var ex = await Should.ThrowAsync<InvalidOperationException>(async () =>
                await response.ReadXmlAsync<SampleModel>());

            ex.InnerException.ShouldBeOfType<XmlException>();
        }

        [Fact]
        public async Task ReadXmlAsync_ThrowsOperationCanceled_WhenTokenIsCanceled()
        {
            var xml = "<SampleModel><Name>Test Name</Name></SampleModel>";
            var response = new HttpResponseMessage
            {
                Content = new StringContent(xml, Encoding.UTF8, "application/xml")
            };

            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Should.ThrowAsync<OperationCanceledException>(async () =>
                await response.ReadXmlAsync<SampleModel>(cts.Token));
        }

        [Fact]
        public async Task ReadXmlAsync_TaskOverload_ReturnsDeserializedInstance_WhenXmlIsValid()
        {
            var xml = "<SampleModel><Name>FromTask</Name></SampleModel>";
            var response = new HttpResponseMessage
            {
                Content = new StringContent(xml, Encoding.UTF8, "application/xml")
            };

            var task = Task.FromResult(response);

            var result = await task.ReadXmlAsync<SampleModel>();

            result.ShouldNotBeNull();
            result!.Name.ShouldBe("FromTask");
        }

        [Fact]
        public async Task ReadXmlAsync_TaskOverload_UsesXmlReaderSettings_WhenProvided()
        {
            var xml = "<SampleModel><Name>FromTaskWithSettings</Name></SampleModel>";
            var response = new HttpResponseMessage
            {
                Content = new StringContent(xml, Encoding.UTF8, "application/xml")
            };

            var task = Task.FromResult(response);
            var settings = new XmlReaderSettings
            {
                IgnoreComments = true
            };

            var result = await task.ReadXmlAsync<SampleModel>(settings);

            result.ShouldNotBeNull();
            result!.Name.ShouldBe("FromTaskWithSettings");
        }

        [Fact]
        public async Task ReadXmlAsync_TaskOverload_ThrowsOperationCanceled_WhenTokenIsCanceled()
        {
            var xml = "<SampleModel><Name>Canceled</Name></SampleModel>";
            var response = new HttpResponseMessage
            {
                Content = new StringContent(xml, Encoding.UTF8, "application/xml")
            };

            var task = Task.FromResult(response);
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Should.ThrowAsync<OperationCanceledException>(async () =>
                await task.ReadXmlAsync<SampleModel>(cts.Token));
        }

        [Fact]
        public async Task ReadXmlAsync_TaskOverload_ThrowsArgumentNullException_WhenResponseTaskIsNull()
        {
            Task<HttpResponseMessage>? task = null;

            await Should.ThrowAsync<ArgumentNullException>(async () =>
                await task!.ReadXmlAsync<SampleModel>());
        }

        [Fact]
        public async Task ReadXmlAsync_ThrowsArgumentNullException_WhenSettingsIsNull()
        {
            var response = new HttpResponseMessage
            {
                Content = new StringContent("<SampleModel />", Encoding.UTF8, "application/xml")
            };

            XmlReaderSettings? settings = null;

            await Should.ThrowAsync<ArgumentNullException>(async () =>
                await response.ReadXmlAsync<SampleModel>(settings!));
        }

        [Fact]
        public async Task ReadXmlAsync_TaskOverload_ThrowsArgumentNullException_WhenSettingsIsNull()
        {
            var response = new HttpResponseMessage
            {
                Content = new StringContent("<SampleModel />", Encoding.UTF8, "application/xml")
            };

            var task = Task.FromResult(response);
            XmlReaderSettings? settings = null;

            await Should.ThrowAsync<ArgumentNullException>(async () =>
                await task.ReadXmlAsync<SampleModel>(settings!));
        }

        [Fact]
        public async Task ReadXmlAsync_UsesSettingsAndToken_WhenBothProvided()
        {
            var xml = "<SampleModel><Name>WithSettingsAndToken</Name></SampleModel>";
            var response = new HttpResponseMessage
            {
                Content = new StringContent(xml, Encoding.UTF8, "application/xml")
            };

            var settings = new XmlReaderSettings
            {
                IgnoreWhitespace = true,
                IgnoreComments = true
            };

            using var cts = new CancellationTokenSource();

            var result = await response.ReadXmlAsync<SampleModel>(settings, cts.Token);

            result.ShouldNotBeNull();
            result!.Name.ShouldBe("WithSettingsAndToken");
        }

        [Fact]
        public async Task ReadXmlAsync_TaskOverload_UsesSettingsAndToken_WhenBothProvided()
        {
            var xml = "<SampleModel><Name>FromTaskWithSettingsAndToken</Name></SampleModel>";
            var response = new HttpResponseMessage
            {
                Content = new StringContent(xml, Encoding.UTF8, "application/xml")
            };

            var task = Task.FromResult(response);

            var settings = new XmlReaderSettings
            {
                IgnoreWhitespace = true,
                IgnoreComments = true
            };

            using var cts = new CancellationTokenSource();

            var result = await task.ReadXmlAsync<SampleModel>(settings, cts.Token);

            result.ShouldNotBeNull();
            result!.Name.ShouldBe("FromTaskWithSettingsAndToken");
        }
    }

    public class ReadXmlElementAsyncTests
    {
        [Fact]
        public async Task ReadXmlElementAsync_ReturnsNull_WhenContentIsNull()
        {
            var response = new HttpResponseMessage
            {
                Content = null
            };

            var result = await response.ReadXmlElementAsync();

            result.ShouldBeNull();
        }

        [Fact]
        public async Task ReadXmlElementAsync_ReturnsNull_WhenContentIsEmpty()
        {
            var response = new HttpResponseMessage
            {
                Content = new StringContent(string.Empty, Encoding.UTF8, "application/xml")
            };

            var result = await response.ReadXmlElementAsync();

            result.ShouldBeNull();
        }

        [Fact]
        public async Task ReadXmlElementAsync_ReturnsNull_WhenContentIsWhitespace()
        {
            var response = new HttpResponseMessage
            {
                Content = new StringContent("\r\n   \t", Encoding.UTF8, "application/xml")
            };

            var result = await response.ReadXmlElementAsync();

            result.ShouldBeNull();
        }

        [Fact]
        public async Task ReadXmlElementAsync_ReturnsXElement_WhenXmlIsValid()
        {
            var xml = "<SampleModel><Name>ElementName</Name></SampleModel>";
            var response = new HttpResponseMessage
            {
                Content = new StringContent(xml, Encoding.UTF8, "application/xml")
            };

            var element = await response.ReadXmlElementAsync();

            element.ShouldNotBeNull();
            element!.Name.LocalName.ShouldBe("SampleModel");
            element.Element("Name")!.Value.ShouldBe("ElementName");
        }

        [Fact]
        public async Task ReadXmlElementAsync_UsesLoadOptions_WhenProvided()
        {
            var xml = "<SampleModel>  <Name>  WithWhitespace  </Name>  </SampleModel>";
            var response = new HttpResponseMessage
            {
                Content = new StringContent(xml, Encoding.UTF8, "application/xml")
            };

            var element = await response.ReadXmlElementAsync(LoadOptions.PreserveWhitespace);

            element.ShouldNotBeNull();
            element!.Element("Name")!.Value.ShouldBe("  WithWhitespace  ");
        }

        [Fact]
        public async Task ReadXmlElementAsync_ThrowsXmlException_WhenXmlIsInvalid()
        {
            var xml = "<SampleModel><Name>Bad</SampleModel>"; // malformed
            var response = new HttpResponseMessage
            {
                Content = new StringContent(xml, Encoding.UTF8, "application/xml")
            };

            await Should.ThrowAsync<XmlException>(async () =>
                await response.ReadXmlElementAsync());
        }

        [Fact]
        public async Task ReadXmlElementAsync_ThrowsOperationCanceled_WhenTokenIsCanceled()
        {
            var xml = "<SampleModel><Name>CancelMe</Name></SampleModel>";
            var response = new HttpResponseMessage
            {
                Content = new StringContent(xml, Encoding.UTF8, "application/xml")
            };

            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Should.ThrowAsync<OperationCanceledException>(async () =>
                await response.ReadXmlElementAsync(cts.Token));
        }

        [Fact]
        public async Task ReadXmlElementAsync_TaskOverload_ReturnsXElement_WhenXmlIsValid()
        {
            var xml = "<SampleModel><Name>FromTaskElement</Name></SampleModel>";
            var response = new HttpResponseMessage
            {
                Content = new StringContent(xml, Encoding.UTF8, "application/xml")
            };

            var task = Task.FromResult(response);

            var element = await task.ReadXmlElementAsync();

            element.ShouldNotBeNull();
            element!.Name.LocalName.ShouldBe("SampleModel");
            element.Element("Name")!.Value.ShouldBe("FromTaskElement");
        }

        [Fact]
        public async Task ReadXmlElementAsync_TaskOverload_UsesLoadOptions_WhenProvided()
        {
            var xml = "<SampleModel>  <Name>  FromTaskWithWhitespace  </Name>  </SampleModel>";
            var response = new HttpResponseMessage
            {
                Content = new StringContent(xml, Encoding.UTF8, "application/xml")
            };

            var task = Task.FromResult(response);

            var element = await task.ReadXmlElementAsync(LoadOptions.PreserveWhitespace);

            element.ShouldNotBeNull();
            element!.Element("Name")!.Value.ShouldBe("  FromTaskWithWhitespace  ");
        }

        [Fact]
        public async Task ReadXmlElementAsync_TaskOverload_ThrowsOperationCanceled_WhenTokenIsCanceled()
        {
            var xml = "<SampleModel><Name>CancelFromTask</Name></SampleModel>";
            var response = new HttpResponseMessage
            {
                Content = new StringContent(xml, Encoding.UTF8, "application/xml")
            };

            var task = Task.FromResult(response);
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Should.ThrowAsync<OperationCanceledException>(async () =>
                await task.ReadXmlElementAsync(cts.Token));
        }

        [Fact]
        public async Task ReadXmlElementAsync_TaskOverload_ThrowsArgumentNullException_WhenResponseTaskIsNull()
        {
            Task<HttpResponseMessage>? task = null;

            await Should.ThrowAsync<ArgumentNullException>(async () =>
                await task!.ReadXmlElementAsync());
        }

        [Fact]
        public async Task ReadXmlElementAsync_UsesOptionsAndToken_WhenBothProvided()
        {
            var xml = "<SampleModel>  <Name>  WithOptionsAndToken  </Name>  </SampleModel>";
            var response = new HttpResponseMessage
            {
                Content = new StringContent(xml, Encoding.UTF8, "application/xml")
            };

            using var cts = new CancellationTokenSource();

            var element = await response.ReadXmlElementAsync(LoadOptions.PreserveWhitespace, cts.Token);

            element.ShouldNotBeNull();
            element!.Element("Name")!.Value.ShouldBe("  WithOptionsAndToken  ");
        }

        [Fact]
        public async Task ReadXmlElementAsync_TaskOverload_UsesOptionsAndToken_WhenBothProvided()
        {
            var xml = "<SampleModel>  <Name>  FromTaskWithOptionsAndToken  </Name>  </SampleModel>";
            var response = new HttpResponseMessage
            {
                Content = new StringContent(xml, Encoding.UTF8, "application/xml")
            };

            var task = Task.FromResult(response);

            using var cts = new CancellationTokenSource();

            var element = await task.ReadXmlElementAsync(LoadOptions.PreserveWhitespace, cts.Token);

            element.ShouldNotBeNull();
            element!.Element("Name")!.Value.ShouldBe("  FromTaskWithOptionsAndToken  ");
        }
    }
}
