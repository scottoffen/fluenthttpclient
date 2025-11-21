using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace FluentHttpClient.Tests;

public class FluentXmlSerializerTests
{
    public class TestDto
    {
        public string? Name { get; set; }
        public int Value { get; set; }
    }

    [Fact]
    public void Serialize_And_Deserialize_RoundTrips_UsingDefaultSettings_WhenValid()
    {
        var dto = new TestDto
        {
            Name = "Foo",
            Value = 42
        };

        var xml = FluentXmlSerializer.Serialize(dto);
        xml.ShouldNotBeNullOrWhiteSpace();

        var trimmed = xml.TrimStart();
        trimmed.StartsWith("<?xml", StringComparison.Ordinal).ShouldBeFalse();

        var doc = XDocument.Parse(xml);
        doc.Root.ShouldNotBeNull();
        doc.Root!.Name.LocalName.ShouldBe(nameof(TestDto));
        doc.Root.Element(nameof(TestDto.Name))?.Value.ShouldBe("Foo");
        doc.Root.Element(nameof(TestDto.Value))?.Value.ShouldBe("42");

        var roundTripped = FluentXmlSerializer.Deserialize<TestDto>(xml);
        roundTripped.ShouldNotBeNull();
        roundTripped.Name.ShouldBe("Foo");
        roundTripped.Value.ShouldBe(42);
    }

    [Fact]
    public void Serialize_ThrowsArgumentNullException_WhenObjectIsNull()
    {
        Should.Throw<ArgumentNullException>(() => FluentXmlSerializer.Serialize<TestDto>(null!));
    }

    [Fact]
    public void Deserialize_ThrowsArgumentNullException_WhenXmlIsNull()
    {
        Should.Throw<ArgumentNullException>(() => FluentXmlSerializer.Deserialize<TestDto>(null!));
    }

    [Fact]
    public void Serialize_IncludesXmlDeclarationWithUtf16Encoding_WhenSettingsSpecifyUtf16()
    {
        var dto = new TestDto
        {
            Name = "Bar",
            Value = 7
        };

        var settings = new XmlWriterSettings
        {
            OmitXmlDeclaration = false,
            Encoding = Encoding.Unicode
        };

        var xml = FluentXmlSerializer.Serialize(dto, settings);

        var trimmed = xml.TrimStart();
        trimmed.StartsWith("<?xml", StringComparison.Ordinal).ShouldBeTrue();
        trimmed.Contains("encoding=\"utf-16\"", StringComparison.OrdinalIgnoreCase).ShouldBeTrue();

        var doc = XDocument.Parse(xml);
        doc.Root.ShouldNotBeNull();
        doc.Root!.Name.LocalName.ShouldBe(nameof(TestDto));
    }

    [Fact]
    public void Serialize_UsesUtf8EncodingInDeclaration_WhenEncodingNotSpecified()
    {
        var dto = new TestDto
        {
            Name = "Baz",
            Value = 99
        };

        var settings = new XmlWriterSettings
        {
            OmitXmlDeclaration = false
            // Encoding left null on purpose
        };

        var xml = FluentXmlSerializer.Serialize(dto, settings);

        var trimmed = xml.TrimStart();
        trimmed.StartsWith("<?xml", StringComparison.Ordinal).ShouldBeTrue();
        trimmed.Contains("encoding=\"utf-8\"", StringComparison.OrdinalIgnoreCase).ShouldBeTrue();

        var doc = XDocument.Parse(xml);
        doc.Root.ShouldNotBeNull();
        doc.Root!.Name.LocalName.ShouldBe(nameof(TestDto));
    }

    [Fact]
    public void Deserialize_IgnoresCommentsAndWhitespaceNodes_UsingDefaultReaderSettings()
    {
        var xml = """
                  <!-- Leading comment -->

                  <TestDto>
                      <!-- Name comment -->
                      <Name>
                          Foo
                      </Name>

                      <!-- Value comment -->
                      <Value>
                          42
                      </Value>
                  </TestDto>
                  """;

        var dto = FluentXmlSerializer.Deserialize<TestDto>(xml);

        dto.ShouldNotBeNull();
        dto.Name.ShouldNotBeNull();
        dto.Name!.Trim().ShouldBe("Foo");
        dto.Value.ShouldBe(42);
    }

    [Fact]
    public void Deserialize_ProhibitsDtdProcessing_ByDefault()
    {
        var xml = """
                  <!DOCTYPE TestDto [
                      <!ELEMENT TestDto (Name, Value)>
                  ]>
                  <TestDto>
                      <Name>Foo</Name>
                      <Value>42</Value>
                  </TestDto>
                  """;

        var ex = Should.Throw<InvalidOperationException>(() => FluentXmlSerializer.Deserialize<TestDto>(xml));

        ex.ShouldNotBeNull();

        var inner = ex.InnerException;
        while (inner?.InnerException is not null)
        {
            inner = inner.InnerException;
        }

        inner.ShouldNotBeNull();
        inner.ShouldBeOfType<XmlException>();
        inner!.Message.ShouldContain("DTD is prohibited", Case.Sensitive);
    }

    [Fact]
    public void Serialize_And_Deserialize_RoundTrips_WithCustomSettings()
    {
        var dto = new TestDto
        {
            Name = "Custom",
            Value = 123
        };

        var writerSettings = new XmlWriterSettings
        {
            OmitXmlDeclaration = false,
            Encoding = Encoding.UTF8,
            Indent = true
        };

        var xml = FluentXmlSerializer.Serialize(dto, writerSettings);
        xml.ShouldNotBeNullOrWhiteSpace();

        var parsed = XDocument.Parse(xml);
        parsed.Root.ShouldNotBeNull();
        parsed.Root!.Element(nameof(TestDto.Name))?.Value.ShouldBe("Custom");
        parsed.Root.Element(nameof(TestDto.Value))?.Value.ShouldBe("123");

        var deserialized = FluentXmlSerializer.Deserialize<TestDto>(xml);
        deserialized.ShouldNotBeNull();
        deserialized.Name.ShouldBe("Custom");
        deserialized.Value.ShouldBe(123);
    }

    [Fact]
    public void SerializerCache_ReusesSerializerInstance_ForSameType()
    {
        var cacheField = typeof(FluentXmlSerializer)
            .GetField("SerializerCache", BindingFlags.NonPublic | BindingFlags.Static);

        cacheField.ShouldNotBeNull();

        var cache = cacheField!.GetValue(null) as ConcurrentDictionary<Type, XmlSerializer>;
        cache.ShouldNotBeNull();

        var dto1 = new TestDto { Name = "One", Value = 1 };
        FluentXmlSerializer.Serialize(dto1);

        cache!.TryGetValue(typeof(TestDto), out var serializer1).ShouldBeTrue();
        serializer1.ShouldNotBeNull();

        var dto2 = new TestDto { Name = "Two", Value = 2 };
        FluentXmlSerializer.Serialize(dto2);

        cache.TryGetValue(typeof(TestDto), out var serializer2).ShouldBeTrue();
        serializer2.ShouldBeSameAs(serializer1);
    }

    [Fact]
    public void DefaultContentType_HasExpectedValue()
    {
        FluentXmlSerializer.DefaultContentType.ShouldBe("application/xml");
    }
}
