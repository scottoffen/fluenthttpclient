using System.Text.Json;

namespace FluentHttpClient;

public static class FluentJsonDeserialization
{
    public static JsonSerializerOptions Options { get; set; } = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<T> DeserializeJsonAsync<T>(this Task<Stream> result)
    {
        return await result.DeserializeJsonAsync<T>(Options);
    }

    public static async Task<T> DeserializeJsonAsync<T>(this Task<Stream> result, JsonSerializerOptions options)
    {
        return await JsonSerializer.DeserializeAsync<T>(await result, options);
    }

    public static async Task<T> DeserializeJsonAsync<T>(this Task<String> result)
    {
        return await result.DeserializeJsonAsync<T>(Options);
    }

    public static async Task<T> DeserializeJsonAsync<T>(this Task<string> result, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<T>(await result, options);
    }
}