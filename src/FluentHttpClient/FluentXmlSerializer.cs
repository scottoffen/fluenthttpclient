using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace FluentHttpClient;

/// <summary>
/// Provides internal XML serialization and deserialization functionality using XmlSerializer.
/// </summary>
/// <remarks>
/// <para>
/// XmlSerializer instances are cached per type to avoid repeated code generation overhead.
/// On .NET Framework and .NET Core versions prior to .NET 5, XmlSerializer generates dynamic assemblies
/// that cannot be unloaded, which may lead to memory accumulation in long-running applications
/// that serialize many different types.
/// </para>
/// <para>
/// For applications concerned about memory usage in scenarios with many types, or for Native AOT compatibility,
/// consider using JSON serialization instead. Alternatively, pre-generate XML serializers using
/// the XML Serializer Generator tool (sgen.exe) or source generators (.NET 6+).
/// </para>
/// </remarks>
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

#if NET7_0_OR_GREATER
    [RequiresDynamicCode("XmlSerializer uses dynamic code generation which is not supported with Native AOT.")]
#endif
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("XML serialization using XmlSerializer may be incompatible with trimming. Ensure all required members are preserved or use these APIs only in non-trimmed scenarios.")]
#endif
    public static T Deserialize<T>(string xml)
        where T : class
        => Deserialize<T>(xml, DefaultReaderSettings);

#if NET7_0_OR_GREATER
    [RequiresDynamicCode("XmlSerializer uses dynamic code generation which is not supported with Native AOT.")]
#endif
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("XML serialization using XmlSerializer may be incompatible with trimming. Ensure all required members are preserved or use these APIs only in non-trimmed scenarios.")]
#endif
    public static T Deserialize<T>(string xml, XmlReaderSettings settings)
        where T : class
    {
        Guard.AgainstNull(xml, nameof(xml));
        Guard.AgainstNull(settings, nameof(settings));

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

#if NET7_0_OR_GREATER
    [RequiresDynamicCode("XmlSerializer uses dynamic code generation which is not supported with Native AOT.")]
#endif
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("XML serialization using XmlSerializer may be incompatible with trimming. Ensure all required members are preserved or use these APIs only in non-trimmed scenarios.")]
#endif
    public static string Serialize<T>(T obj)
        where T : class
        => Serialize(obj, DefaultWriterSettings);

#if NET7_0_OR_GREATER
    [RequiresDynamicCode("XmlSerializer uses dynamic code generation which is not supported with Native AOT.")]
#endif
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("XML serialization using XmlSerializer may be incompatible with trimming. Ensure all required members are preserved or use these APIs only in non-trimmed scenarios.")]
#endif
    public static string Serialize<T>(T obj, XmlWriterSettings settings)
        where T : class
    {
        Guard.AgainstNull(obj, nameof(obj));

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

/// <summary>
/// A specialized <see cref="StringWriter"/> that supports custom encoding for XML serialization.
/// </summary>
internal sealed class XmlStringWriter : StringWriter
{
    private readonly Encoding _encoding;

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlStringWriter"/> class with the specified format provider and encoding.
    /// </summary>
    /// <param name="formatProvider">An object that controls formatting.</param>
    /// <param name="encoding">The character encoding to use.</param>
    public XmlStringWriter(IFormatProvider formatProvider, Encoding encoding)
        : base(formatProvider)
    {
        _encoding = encoding;
    }

    /// <summary>
    /// Gets the encoding for this string writer.
    /// </summary>
    public override Encoding Encoding => _encoding;
}
