using System.Text.Json;
using System.Text.Json.Serialization;

#if NET5_0_OR_GREATER
using System.Net.Http.Json;
#endif

namespace FluentHttpClient;

public static class RestRequestJsonExtensions
{
    private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

#if NET5_0_OR_GREATER
    public static RestRequestBuilder WithJsonContent(this RestRequestBuilder builder, object content)
    {
        return builder.WithJsonContent(content, _options);
    }

    public static RestRequestBuilder WithJsonContent(this RestRequestBuilder builder, object content, JsonSerializerOptions options)
    {
        builder.Content = JsonContent.Create(content, options: options);
        return builder;
    }
#endif
}