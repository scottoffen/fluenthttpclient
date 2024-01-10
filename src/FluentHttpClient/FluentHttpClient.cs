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

    /// <summary>
    /// When true, query parameters that are null or empty will not be included when converted to a string.
    /// </summary>
    public static bool RemoveEmptyQueryParameters { get; set; } = false;
}
