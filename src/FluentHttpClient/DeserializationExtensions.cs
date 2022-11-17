using System.Text.Json;

namespace FluentHttpClient;

public static class FluentJsonDeserialization
{
    public static async Task<T> DeserializeJsonAsync<T>(this Task<Stream> result)
    {
        return await result.DeserializeJsonAsync<T>(FluentHttpClient.DefaultJsonSerializerOptions);
    }

    public static async Task<T> DeserializeJsonAsync<T>(this Task<Stream> result, JsonSerializerOptions options)
    {
        return await JsonSerializer.DeserializeAsync<T>(await result, options);
    }

    public static async Task<T> DeserializeJsonAsync<T>(this Task<String> result)
    {
        return await result.DeserializeJsonAsync<T>(FluentHttpClient.DefaultJsonSerializerOptions);
    }

    public static async Task<T> DeserializeJsonAsync<T>(this Task<string> result, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<T>(await result, options);
    }

    public static async Task<T> DeserializeJsonAsync<T>(this Task<HttpResponseMessage> result)
    {
        return await result.DeserializeJsonAsync<T>(FluentHttpClient.DefaultJsonSerializerOptions);
    }

    public static async Task<T> DeserializeJsonAsync<T>(this Task<HttpResponseMessage> taskResponse, JsonSerializerOptions options)
    {
        var response = await taskResponse;
        return JsonSerializer.Deserialize<T>(await response.GetResponseStreamAsync(), options);
    }

    public static async Task<T> DeserializeJsonAsync<T>(this Task<HttpResponseMessage> result, Func<HttpResponseMessage, T> defaultAction)
    {
        return await result.DeserializeJsonAsync<T>(defaultAction, FluentHttpClient.DefaultJsonSerializerOptions);
    }

    public static async Task<T> DeserializeJsonAsync<T>(this Task<HttpResponseMessage> taskResponse, Func<HttpResponseMessage, T> defaultAction, JsonSerializerOptions options)
    {
        var response = await taskResponse;
        return (response.IsSuccessStatusCode)
            ? JsonSerializer.Deserialize<T>(await response.GetResponseStreamAsync(), options)
            : defaultAction(response);
    }

    public static async Task<T> DeserializeJsonAsync<T>(this Task<HttpResponseMessage> result, Func<HttpResponseMessage, Task<T>> defaultAction)
    {
        return await result.DeserializeJsonAsync<T>(defaultAction, FluentHttpClient.DefaultJsonSerializerOptions);
    }

    public static async Task<T> DeserializeJsonAsync<T>(this Task<HttpResponseMessage> taskResponse, Func<HttpResponseMessage, Task<T>> defaultAction, JsonSerializerOptions options)
    {
        var response = await taskResponse;
        return (response.IsSuccessStatusCode)
            ? JsonSerializer.Deserialize<T>(await response.GetResponseStreamAsync(), options)
            : await defaultAction(response);
    }
}
