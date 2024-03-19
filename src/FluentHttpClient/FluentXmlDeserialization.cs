using System.Xml.Linq;
using System.Xml.Serialization;

namespace FluentHttpClient;

/// <summary>
/// Extension methods for XML deserialization.
/// </summary>
public static class FluentXmlDeserialization
{
    /// <summary>
    /// Asynchronously creates a new XElement and initializes its underlying XML tree using the stream from the <see cref="HttpResponseMessage.Content"/>.
    /// </summary>
    /// <param name="taskResponse"></param>
    /// <returns>A new XElement containing the contents of the <see cref="HttpResponseMessage.Content"/> stream.</returns>
    public static async Task<XElement> DeserializeXmlAsync(this Task<HttpResponseMessage> taskResponse)
    {
        return await taskResponse
            .DeserializeXmlAsync(LoadOptions.None, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously creates a new XElement and initializes its underlying XML tree using the stream from the <see cref="HttpResponseMessage.Content"/>.
    /// </summary>
    /// <param name="response"></param>
    /// <returns>A new XElement containing the contents of the <see cref="HttpResponseMessage.Content"/> stream.</returns>
    public static async Task<XElement> DeserializeXmlAsync(this HttpResponseMessage response)
    {
        return await response
            .DeserializeXmlAsync(LoadOptions.None, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously creates a new XElement and initializes its underlying XML tree using the stream from the <see cref="HttpResponseMessage.Content"/>, optionally preserving white space.
    /// </summary>
    /// <param name="taskResponse"></param>
    /// <param name="options"></param>
    /// <returns>A new XElement containing the contents of the <see cref="HttpResponseMessage.Content"/> stream.</returns>
    public static async Task<XElement> DeserializeXmlAsync(this Task<HttpResponseMessage> taskResponse, LoadOptions options)
    {
        return await taskResponse
            .DeserializeXmlAsync(options, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously creates a new XElement and initializes its underlying XML tree using the stream from the <see cref="HttpResponseMessage.Content"/>, optionally preserving white space.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="options"></param>
    /// <returns>A new XElement containing the contents of the <see cref="HttpResponseMessage.Content"/> stream.</returns>
    public static async Task<XElement> DeserializeXmlAsync(this HttpResponseMessage response, LoadOptions options)
    {
        return await response
            .DeserializeXmlAsync(options, CancellationToken.None)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously creates a new XElement and initializes its underlying XML tree using the stream from the <see cref="HttpResponseMessage.Content"/>.
    /// </summary>
    /// <param name="taskResponse"></param>
    /// <param name="token"></param>
    /// <returns>A new XElement containing the contents of the <see cref="HttpResponseMessage.Content"/> stream.</returns>
    public static async Task<XElement> DeserializeXmlAsync(this Task<HttpResponseMessage> taskResponse, CancellationToken token)
    {
        return await taskResponse
            .DeserializeXmlAsync(LoadOptions.None, token)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously creates a new XElement and initializes its underlying XML tree using the stream from the <see cref="HttpResponseMessage.Content"/>.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="token"></param>
    /// <returns>A new XElement containing the contents of the <see cref="HttpResponseMessage.Content"/> stream.</returns>
    public static async Task<XElement> DeserializeXmlAsync(this HttpResponseMessage response, CancellationToken token)
    {
        return await response
            .DeserializeXmlAsync(LoadOptions.None, token)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously creates a new XElement and initializes its underlying XML tree using the stream from the <see cref="HttpResponseMessage.Content"/>, optionally preserving white space.
    /// </summary>
    /// <param name="taskResponse"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    /// <returns>A new XElement containing the contents of the <see cref="HttpResponseMessage.Content"/> stream.</returns>
    public static async Task<XElement> DeserializeXmlAsync(this Task<HttpResponseMessage> taskResponse, LoadOptions options, CancellationToken token)
    {
        var response = await taskResponse;
        return await response
            .DeserializeXmlAsync(options, token)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously creates a new XElement and initializes its underlying XML tree using the stream from the <see cref="HttpResponseMessage.Content"/>, optionally preserving white space.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="options"></param>
    /// <param name="token"></param>
    /// <returns>A new XElement containing the contents of the <see cref="HttpResponseMessage.Content"/> stream.</returns>
    public static async Task<XElement> DeserializeXmlAsync(this HttpResponseMessage response, LoadOptions options, CancellationToken token)
    {   
        var stream = await response.GetResponseStreamAsync();
        return await XElement
            .LoadAsync(stream, options, token)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Deserializes the XML document contained in <see cref="HttpResponseMessage.Content"/> to an instance of type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="taskResponse"></param>
    /// <param name="defaultAction"></param>
    /// <returns></returns>
    /// <returns>The object being deserialized.</returns>
    /// <exception cref="InvalidOperationException" />
    public static async Task<T?> DeserializeXmlAsync<T>(this Task<HttpResponseMessage> taskResponse, Func<HttpResponseMessage, Task<T>>? defaultAction)
    {
        var response = await taskResponse;
        if (!response.IsSuccessStatusCode)
        {
            if (defaultAction == null) return default;
            return await defaultAction(response);
        }

        return await response.DeserializeXmlAsync<T>().ConfigureAwait(false);
    }

    /// <summary>
    /// Deserializes the XML document contained in <see cref="HttpResponseMessage.Content"/> to an instance of type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="response"></param>
    /// <returns>The object being deserialized.</returns>
    /// <exception cref="InvalidOperationException" />
    public static async Task<T?> DeserializeXmlAsync<T>(this HttpResponseMessage response)
    {
        using var reader = new StreamReader(await response.GetResponseStreamAsync());
        var serializer = new XmlSerializer(typeof(T));
        return (T?)serializer.Deserialize(reader);
    }
}
