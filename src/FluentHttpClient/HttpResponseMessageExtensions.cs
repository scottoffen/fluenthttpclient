using System.Text;

namespace FluentHttpClient;

public static class HttpResponseMessageExtensions
{
    public static async Task<string> GetResponseStringAsync(this HttpResponseMessage responseMessage)
    {
        return (responseMessage.Content != null)
            ? await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false)
            : responseMessage.StatusCode.ToString();
    }

    public static async Task<string> GetResponseStringAsync(this Task<HttpResponseMessage> taskResponseMessage)
    {
        var responseMessage = await taskResponseMessage;
        return await responseMessage.GetResponseStringAsync().ConfigureAwait(false);
    }

    public static async Task<Stream> GetResponseStreamAsync(this HttpResponseMessage responseMessage)
    {
        return (responseMessage.Content != null)
            ? await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false)
            : new MemoryStream(Encoding.UTF8.GetBytes(responseMessage.StatusCode.ToString()));
    }

    public static async Task<Stream> GetResponseStreamAsync(this Task<HttpResponseMessage> taskResponseMessage)
    {
        var responseMessage = await taskResponseMessage;
        return await responseMessage.GetResponseStreamAsync().ConfigureAwait(false);
    }

    public static async Task<byte[]> GetResponseBytesAsync(this HttpResponseMessage responseMessage)
    {
        return (responseMessage.Content != null)
            ? await responseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false)
            : Encoding.UTF8.GetBytes(responseMessage.StatusCode.ToString());
    }

    public static async Task<byte[]> GetResponseBytesAsync(this Task<HttpResponseMessage> taskResponseMessage)
    {
        var responseMessage = await taskResponseMessage;
        return await responseMessage.GetResponseBytesAsync().ConfigureAwait(false);
    }
}