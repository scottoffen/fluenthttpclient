using System.Xml.Linq;
using System.Xml.Serialization;

namespace FluentHttpClient;

public static class FluentXmlDeserialization
{
    public static async Task<XElement> DeserializeXmlAsync(this Task<HttpResponseMessage> taskResponse)
    {
        return await taskResponse.DeserializeXmlAsync(LoadOptions.None, CancellationToken.None);
    }

    public static async Task<XElement> DeserializeXmlAsync(this Task<HttpResponseMessage> taskResponse, LoadOptions options)
    {
        return await taskResponse.DeserializeXmlAsync(options, CancellationToken.None);
    }

    public static async Task<XElement> DeserializeXmlAsync(this Task<HttpResponseMessage> taskResponse, LoadOptions options, CancellationToken? token)
    {
        token ??= CancellationToken.None;
        var response = await taskResponse;

        return await XElement.LoadAsync(await response.GetResponseStreamAsync(), options, token.Value);
    }

    public static async Task<T> DeserializeXmlAsync<T>(this Task<HttpResponseMessage> taskResponse, Func<HttpResponseMessage, Task<T>> defaultAction)
    {
        var response = await taskResponse;
        return (response.IsSuccessStatusCode)
            ? await response.DeserializeXmlAsync<T>()
            : await defaultAction(response);
    }

    public static async Task<T> DeserializeXmlAsync<T>(this Task<HttpResponseMessage> taskResponse)
    {
        var response = await taskResponse;
        return await response.DeserializeXmlAsync<T>();
    }

    public static async Task<T> DeserializeXmlAsync<T>(this HttpResponseMessage response)
    {
        using var reader = new StreamReader(await response.GetResponseStreamAsync());
        var serializer = new XmlSerializer(typeof(T));
        return (T)serializer.Deserialize(reader);
    }
}
