using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace FluentHttpClient;

/// <summary>
/// Fluent extension methods for deserializing XML content from an <see cref="HttpResponseMessage"/>.
/// </summary>
#if NET7_0_OR_GREATER
[RequiresDynamicCode("XmlSerializer uses dynamic code generation which is not supported with Native AOT.")]
#endif
#if NET6_0_OR_GREATER
[RequiresUnreferencedCode("XML serialization using XmlSerializer may be incompatible with trimming. Ensure all required members are preserved or use these APIs only in non-trimmed scenarios.")]
#endif
public static class FluentXmlTypedDeserialization
{
    /// <summary>
    /// Reads the response content as XML and deserializes it into the specified type.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the XML content into.</typeparam>
    /// <param name="response">The HTTP response whose content will be read.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized object, or null if the content is empty.</returns>
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
    /// <typeparam name="T">The type to deserialize the XML content into.</typeparam>
    /// <param name="response">The HTTP response whose content will be read.</param>
    /// <param name="settings">The XML reader settings to use during deserialization.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized object, or null if the content is empty.</returns>
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
    /// <typeparam name="T">The type to deserialize the XML content into.</typeparam>
    /// <param name="response">The HTTP response whose content will be read.</param>
    /// <param name="token">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized object, or null if the content is empty.</returns>
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
    /// <typeparam name="T">The type to deserialize the XML content into.</typeparam>
    /// <param name="response">The HTTP response whose content will be read.</param>
    /// <param name="settings">The XML reader settings to use during deserialization.</param>
    /// <param name="token">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized object, or null if the content is empty.</returns>
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
    /// <typeparam name="T">The type to deserialize the XML content into.</typeparam>
    /// <param name="responseMessage">A task that produces the HTTP response whose content will be read.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized object, or null if the content is empty.</returns>
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
    /// <typeparam name="T">The type to deserialize the XML content into.</typeparam>
    /// <param name="responseMessage">A task that produces the HTTP response whose content will be read.</param>
    /// <param name="settings">The XML reader settings to use during deserialization.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized object, or null if the content is empty.</returns>
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
    /// <typeparam name="T">The type to deserialize the XML content into.</typeparam>
    /// <param name="responseMessage">A task that produces the HTTP response whose content will be read.</param>
    /// <param name="token">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized object, or null if the content is empty.</returns>
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
    /// <typeparam name="T">The type to deserialize the XML content into.</typeparam>
    /// <param name="responseMessage">A task that produces the HTTP response whose content will be read.</param>
    /// <param name="settings">The XML reader settings to use during deserialization.</param>
    /// <param name="token">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized object, or null if the content is empty.</returns>
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
            return null;
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
