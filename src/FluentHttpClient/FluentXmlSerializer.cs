using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace FluentHttpClient;

internal static class FluentXmlSerializer
{
    private static readonly ConcurrentDictionary<Type, XmlSerializer> SerializerCache = new();

    private static readonly XmlReaderSettings DefaultReaderSettings = new()
    {
        CheckCharacters = true,
        IgnoreComments = true,
        IgnoreWhitespace = true,
        DtdProcessing = DtdProcessing.Prohibit
    };

    private static readonly XmlWriterSettings DefaultWriterSettings = new()
    {
        OmitXmlDeclaration = true,
        NewLineChars = Environment.NewLine,
        ConformanceLevel = ConformanceLevel.Document,
        CheckCharacters = true,
    };

    public static readonly string DefaultContentType = "application/xml";

    public static T Deserialize<T>(string xml)
        where T : class
        => Deserialize<T>(xml, DefaultReaderSettings);

    public static T Deserialize<T>(string xml, XmlReaderSettings settings)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(xml);
        ArgumentNullException.ThrowIfNull(settings);

        var serializer = SerializerCache.GetOrAdd(typeof(T), t => new XmlSerializer(t));

        using var stringReader = new StringReader(xml);
        using var xmlReader = XmlReader.Create(stringReader, settings);

        var result = serializer.Deserialize(xmlReader);
        if (result is null)
        {
            throw new InvalidOperationException(
                $"Deserialization of type '{typeof(T)}' returned null.");
        }

        if (result is not T typed)
        {
            throw new InvalidOperationException(
                $"Deserialized object is not of type '{typeof(T)}'. Actual type: '{result.GetType()}'.");
        }

        return typed;
    }

    public static string Serialize<T>(T obj)
        where T : class
        => Serialize(obj, DefaultWriterSettings);

    public static string Serialize<T>(T obj, XmlWriterSettings settings)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(obj);

        var serializer = SerializerCache.GetOrAdd(typeof(T), t => new XmlSerializer(t));
        var encoding = settings.Encoding ?? Encoding.UTF8;

        using var stringWriter = new XmlStringWriter(CultureInfo.InvariantCulture, encoding);
        using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
        {
            serializer.Serialize(xmlWriter, obj);
        }

        return stringWriter.ToString();
    }
}

internal sealed class XmlStringWriter : StringWriter
{
    private readonly Encoding _encoding;

    public XmlStringWriter(IFormatProvider formatProvider, Encoding encoding)
        : base(formatProvider)
    {
        _encoding = encoding;
    }

    public override Encoding Encoding => _encoding;
}
