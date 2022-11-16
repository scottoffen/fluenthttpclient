using System.Text.Json;
using System.Text.Json.Serialization;

namespace FluentHttpClient;

public static class FluentHttpClient
{
    public static JsonSerializerOptions DefaultJsonSerializerOptions { get; set; } = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}
