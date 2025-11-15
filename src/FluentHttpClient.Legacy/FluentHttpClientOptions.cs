using System.Text.Json;
using System.Text.Json.Serialization;

namespace FluentHttpClient;

/// <summary>
/// Global options used by <see cref="FluentHttpClient"/>.
/// </summary>
public static class FluentHttpClientOptions
{
    /// <summary>
    /// Default JsonSerializerOptions that will be use when serializing and deserializing if none are provided.
    /// </summary>
    public static JsonSerializerOptions DefaultJsonSerializerOptions { get; set; } = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Get or set a boolean value that indicate whether query parameters that are null or empty should be removed when converted to a string.
    /// </summary>
    /// <remarks>This property is no longer used. Use the extension method WithQueryParamIfNotNull() instead.</remarks>
    [Obsolete("This property is no longer used. Use the extension method WithQueryParamIfNotNull() instead.")]
    public static bool RemoveEmptyQueryParameters { get; set; } = false;
}
