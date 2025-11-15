using System.Xml;
using System.Xml.Serialization;

namespace FluentHttpClient;

/// <summary>
/// Methods use to serialize objects of specified types into XML documents.
/// </summary>
public static class FluentXmlSerializer
{
    /// <summary>
    /// The default set of features to support on the XmlWriter object used to serialize an object to an XML document.
    /// </summary>
    public static readonly XmlWriterSettings DefaultSettings = new()
    {
        NewLineChars = Environment.NewLine,
        ConformanceLevel = ConformanceLevel.Document,
        CheckCharacters = true,
        Indent = true,
    };

    /// <summary>
    /// The default value used for the Content-Type header for Xml.
    /// </summary>
    public static readonly string DefaultContentType = "application/xml";

    /// <summary>
    /// Asynchronously serializes an object of the specified type to an XML document.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <remarks>Uses the default XmlWriterSettings from <see cref="DefaultSettings"/>.</remarks>
    public static async Task<string> SerializeAsync<T>(T obj)
    {
        return await SerializeAsync(obj, DefaultSettings).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously serializes an object of the specified type to an XML document.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="settings"></param>
    public static async Task<string> SerializeAsync<T>(T obj, XmlWriterSettings settings)
    {
        var serializer = new XmlSerializer(typeof(T));

        using var stream = new MemoryStream();
        using var writer = XmlWriter.Create(stream, settings);
        
        await Task.Run(() => serializer.Serialize(writer, obj));

        stream.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Serializes an object of the specified type to an XML document.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <remarks>Uses the default XmlWriterSettings from <see cref="DefaultSettings"/>.</remarks>
    public static string Serialize<T>(T obj)
    {
        return Serialize(obj, DefaultSettings);
    }

    /// <summary>
    /// Serializes an object of the specified type to an XML document.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="settings"></param>
    public static string Serialize<T>(T obj, XmlWriterSettings settings)
    {
        var serializer = new XmlSerializer(typeof(T));

        using var stream = new MemoryStream();
        using var writer = XmlWriter.Create(stream, settings);
        
        serializer.Serialize(writer, obj);

        stream.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
