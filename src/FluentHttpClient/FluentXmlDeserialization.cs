using System.Xml;
using System.Xml.Linq;

namespace FluentHttpClient;

/// <summary>
/// Fluent extension methods for deserializing XML content from an <see cref="HttpResponseMessage"/>.
/// </summary>
/// <remarks>
/// These methods read the response content as a string and deserialize it either into an
/// concrete type using <see cref="FluentXmlSerializer"/> or into an <see cref="XElement"/>.
/// Empty or whitespace content returns <c>null</c>. Malformed XML results in the underlying
/// XML parsing or deserialization exception being thrown.
/// </remarks>
public static class FluentXmlDeserialization
{
    /// <summary>
    /// Reads the response content as XML and deserializes it into the specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="response"></param>
    public static Task<T?> ReadXmlAsync<T>(
        this HttpResponseMessage response)
        where T : class
    {
        return response.ReadXmlInternalAsync<T>(null, CancellationToken.None);
    }

    /// <summary>
    /// Reads the response content as XML and deserializes it into the specified type
    /// using the provided <see cref="XmlReaderSettings"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="response"></param>
    /// <param name="settings"></param>
    public static Task<T?> ReadXmlAsync<T>(
        this HttpResponseMessage response,
        XmlReaderSettings settings)
        where T : class
    {
        Guard.AgainstNull(settings, nameof(settings));
        return response.ReadXmlInternalAsync<T>(settings, CancellationToken.None);
    }

    /// <summary>
    /// Reads the response content as XML and deserializes it into the specified type,
    /// honoring the provided <see cref="CancellationToken"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="response"></param>
    /// <param name="token"></param>
    public static Task<T?> ReadXmlAsync<T>(
        this HttpResponseMessage response,
        CancellationToken token)
        where T : class
    {
        return response.ReadXmlInternalAsync<T>(null, token);
    }

    /// <summary>
    /// Reads the response content as XML and deserializes it into the specified type
    /// using the provided <see cref="XmlReaderSettings"/> and <see cref="CancellationToken"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="response"></param>
    /// <param name="settings"></param>
    /// <param name="token"></param>
    public static Task<T?> ReadXmlAsync<T>(
        this HttpResponseMessage response,
        XmlReaderSettings settings,
        CancellationToken token)
        where T : class
    {
        Guard.AgainstNull(settings, nameof(settings));
        return response.ReadXmlInternalAsync<T>(settings, token);
    }

    /// <summary>
    /// Awaits the response task, then reads and deserializes the XML content into the specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessage"></param>
    public static Task<T?> ReadXmlAsync<T>(
        this Task<HttpResponseMessage> responseMessage)
        where T : class
    {
        Guard.AgainstNull(responseMessage, nameof(responseMessage));
        return responseMessage.ReadXmlInternalAsync<T>(null, CancellationToken.None);
    }

    /// <summary>
    /// Awaits the response task, then reads and deserializes the XML content into the specified type
    /// using the provided <see cref="XmlReaderSettings"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessage"></param>
    /// <param name="settings"></param>
    public static Task<T?> ReadXmlAsync<T>(
        this Task<HttpResponseMessage> responseMessage,
        XmlReaderSettings settings)
        where T : class
    {
        Guard.AgainstNull(responseMessage, nameof(responseMessage));
        Guard.AgainstNull(settings, nameof(settings));
        return responseMessage.ReadXmlInternalAsync<T>(settings, CancellationToken.None);
    }

    /// <summary>
    /// Awaits the response task, then reads and deserializes the XML content into the specified type,
    /// honoring the provided <see cref="CancellationToken"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessage"></param>
    /// <param name="token"></param>
    public static Task<T?> ReadXmlAsync<T>(
        this Task<HttpResponseMessage> responseMessage,
        CancellationToken token)
        where T : class
    {
        Guard.AgainstNull(responseMessage, nameof(responseMessage));
        return responseMessage.ReadXmlInternalAsync<T>(null, token);
    }

    /// <summary>
    /// Awaits the response task, then reads and deserializes the XML content into the specified type
    /// using the provided <see cref="XmlReaderSettings"/> and <see cref="CancellationToken"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessage"></param>
    /// <param name="settings"></param>
    /// <param name="token"></param>
    public static Task<T?> ReadXmlAsync<T>(
        this Task<HttpResponseMessage> responseMessage,
        XmlReaderSettings settings,
        CancellationToken token)
        where T : class
    {
        Guard.AgainstNull(responseMessage, nameof(responseMessage));
        Guard.AgainstNull(settings, nameof(settings));
        return responseMessage.ReadXmlInternalAsync<T>(settings, token);
    }

    /// <summary>
    /// Reads the response content as XML and parses it into an <see cref="XElement"/>.
    /// </summary>
    /// <param name="response"></param>
    public static Task<XElement?> ReadXmlElementAsync(
        this HttpResponseMessage response)
    {
        return response.ReadXmlElementInternalAsync(LoadOptions.None, CancellationToken.None);
    }

    /// <summary>
    /// Reads the response content as XML and parses it into an <see cref="XElement"/>
    /// using the provided <see cref="LoadOptions"/>.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="options"></param>
    public static Task<XElement?> ReadXmlElementAsync(
        this HttpResponseMessage response,
        LoadOptions options)
    {
        return response.ReadXmlElementInternalAsync(options, CancellationToken.None);
    }

    /// <summary>
    /// Reads the response content as XML and parses it into an <see cref="XElement"/>,
    /// honoring the provided <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="token"></param>
    public static Task<XElement?> ReadXmlElementAsync(
        this HttpResponseMessage response,
        CancellationToken token)
    {
        return response.ReadXmlElementInternalAsync(LoadOptions.None, token);
    }

    /// <summary>
    /// Reads the response content as XML and parses it into an <see cref="XElement"/>
    /// using the provided <see cref="LoadOptions"/> and <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    public static Task<XElement?> ReadXmlElementAsync(
        this HttpResponseMessage response,
        LoadOptions options,
        CancellationToken token)
    {
        return response.ReadXmlElementInternalAsync(options, token);
    }

    /// <summary>
    /// Awaits the response task, then reads and parses the XML content into an <see cref="XElement"/>.
    /// </summary>
    /// <param name="responseMessage"></param>
    public static Task<XElement?> ReadXmlElementAsync(
        this Task<HttpResponseMessage> responseMessage)
    {
        Guard.AgainstNull(responseMessage, nameof(responseMessage));
        return responseMessage.ReadXmlElementInternalAsync(LoadOptions.None, CancellationToken.None);
    }

    /// <summary>
    /// Awaits the response task, then reads and parses the XML content into an <see cref="XElement"/>
    /// using the provided <see cref="LoadOptions"/>.
    /// </summary>
    /// <param name="responseMessage"></param>
    /// <param name="options"></param>
    public static Task<XElement?> ReadXmlElementAsync(
        this Task<HttpResponseMessage> responseMessage,
        LoadOptions options)
    {
        Guard.AgainstNull(responseMessage, nameof(responseMessage));
        return responseMessage.ReadXmlElementInternalAsync(options, CancellationToken.None);
    }

    /// <summary>
    /// Awaits the response task, then reads and parses the XML content into an <see cref="XElement"/>,
    /// honoring the provided <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="responseMessage"></param>
    /// <param name="token"></param>
    public static Task<XElement?> ReadXmlElementAsync(
        this Task<HttpResponseMessage> responseMessage,
        CancellationToken token)
    {
        Guard.AgainstNull(responseMessage, nameof(responseMessage));
        return responseMessage.ReadXmlElementInternalAsync(LoadOptions.None, token);
    }

    /// <summary>
    /// Awaits the response task, then reads and parses the XML content into an <see cref="XElement"/>
    /// using the provided <see cref="LoadOptions"/> and <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="responseMessage"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    public static Task<XElement?> ReadXmlElementAsync(
        this Task<HttpResponseMessage> responseMessage,
        LoadOptions options,
        CancellationToken token)
    {
        Guard.AgainstNull(responseMessage, nameof(responseMessage));
        return responseMessage.ReadXmlElementInternalAsync(options, token);
    }

    private static async Task<XElement?> ReadXmlElementInternalAsync(
        this HttpResponseMessage response,
        LoadOptions options,
        CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        var content = await response.ReadContentAsStringAsync(token).ConfigureAwait(false);

        return !string.IsNullOrWhiteSpace(content)
            ? XElement.Parse(content, options)
            : null;
    }

    private static async Task<XElement?> ReadXmlElementInternalAsync(
        this Task<HttpResponseMessage> responseMessage,
        LoadOptions options,
        CancellationToken token)
    {
        var response = await responseMessage.ConfigureAwait(false);
        return await response.ReadXmlElementInternalAsync(options, token).ConfigureAwait(false);
    }

    private static async Task<T?> ReadXmlInternalAsync<T>(
        this HttpResponseMessage response,
        XmlReaderSettings? settings,
        CancellationToken token)
        where T : class
    {
        token.ThrowIfCancellationRequested();
        var content = await response.ReadContentAsStringAsync(token).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(content))
        {
            return default;
        }

        return settings is not null
            ? FluentXmlSerializer.Deserialize<T>(content, settings)
            : FluentXmlSerializer.Deserialize<T>(content);
    }

    private static async Task<T?> ReadXmlInternalAsync<T>(
        this Task<HttpResponseMessage> responseMessage,
        XmlReaderSettings? settings,
        CancellationToken token)
        where T : class
    {
        var response = await responseMessage.ConfigureAwait(false);
        return await response.ReadXmlInternalAsync<T>(settings, token).ConfigureAwait(false);
    }
}
