using System.Xml.Linq;

namespace FluentHttpClient;

/// <summary>
/// Fluent extension methods for deserializing XML content from an <see cref="HttpResponseMessage"/>.
/// </summary>
public static class FluentXmlDeserialization
{
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
}
