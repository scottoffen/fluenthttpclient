using System.Text.Json.Serialization;

namespace FluentHttpClient.Tests;

internal sealed class SampleModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

internal sealed class OtherModel
{
    public string? Value { get; set; }
}

[JsonSerializable(typeof(SampleModel))]
internal partial class SampleModelJsonContext : JsonSerializerContext
{
}

[JsonSerializable(typeof(OtherModel))]
internal partial class OtherModelJsonContext : JsonSerializerContext
{
}
