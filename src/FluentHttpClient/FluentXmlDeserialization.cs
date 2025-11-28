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
    /// <param name="response">The HTTP response whose content will be read.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the parsed XElement, or null if the content is empty.</returns>
    public static Task<XElement?> ReadXmlElementAsync(
        this HttpResponseMessage response)
    {
        return response.ReadXmlElementInternalAsync(LoadOptions.None, CancellationToken.None);
    }

    /// <summary>
    /// Reads the response content as XML and parses it into an <see cref="XElement"/>
    /// using the provided <see cref="LoadOptions"/>.
    /// </summary>
    /// <param name="response">The HTTP response whose content will be read.</param>
    /// <param name="options">Options that control how the XML is loaded.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the parsed XElement, or null if the content is empty.</returns>
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
    /// <param name="response">The HTTP response whose content will be read.</param>
    /// <param name="token">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the parsed XElement, or null if the content is empty.</returns>
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
    /// <param name="response">The HTTP response whose content will be read.</param>
    /// <param name="options">Options that control how the XML is loaded.</param>
    /// <param name="token">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the parsed XElement, or null if the content is empty.</returns>
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
    /// <param name="responseMessage">A task that produces the HTTP response whose content will be read.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the parsed XElement, or null if the content is empty.</returns>
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
    /// <param name="responseMessage">A task that produces the HTTP response whose content will be read.</param>
    /// <param name="options">Options that control how the XML is loaded.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the parsed XElement, or null if the content is empty.</returns>
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
    /// <param name="responseMessage">A task that produces the HTTP response whose content will be read.</param>
    /// <param name="token">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the parsed XElement, or null if the content is empty.</returns>
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
    /// <param name="responseMessage">A task that produces the HTTP response whose content will be read.</param>
    /// <param name="options">Options that control how the XML is loaded.</param>
    /// <param name="token">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the parsed XElement, or null if the content is empty.</returns>
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
