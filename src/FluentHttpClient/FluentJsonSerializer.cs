using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FluentHttpClient;

/// <summary>
/// Provides default JSON serialization settings and constants for the FluentHttpClient library.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class FluentJsonSerializer
{
    public static readonly string DefaultContentType = "application/json";

    public static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
#if NETSTANDARD2_1_OR_GREATER
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
#endif
    };
}
