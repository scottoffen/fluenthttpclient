---
sidebar_position: 15
title: Native AOT Support
---

FluentHttpClient provides a set of AOT-friendly JSON extensions that work with `System.Text.Json` source generation. These overloads avoid reflection-based serialization and deserialization so you can safely trim and compile your applications ahead of time. The `JsonTypeInfo<T>` overloads are slightly faster and more explicit, while the `JsonSerializerContext` overloads allow passing a shared context instance.

:::danger XML AOT Support

The XML reflection-based serialization and deserialization helpers provided in FluentHttpClient are **not** AOT-compatible and should only be used in JIT-compiled applications.

:::

## Requirements

* .NET 7.0 or later.
* `System.Text.Json` source generation (`JsonSerializerContext` and `JsonTypeInfo<T>`).
* Generated context that includes metadata for the types you serialize/deserialize.

## Example Models

The examples in the following section use this model.

```csharp
public sealed class SampleModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

// Generate AOT metadata for SampleModel
[JsonSerializable(typeof(SampleModel))]
public partial class SampleModelJsonContext : JsonSerializerContext
{
}
```

## Add JSON Content

The AOT-safe JSON content extensions use `JsonTypeInfo<T>` or `JsonSerializerContext`.

```csharp
var model = new SampleModel { Id = 42, Name = "Grant" };

// using JsonTypeInfo<T>
var builder = client
    .UsingBase()
    .WithRoute("api/sample")
    .WithJsonContent(model, SampleModelJsonContext.Default.SampleModel);

// using JsonSerializerContext
var builder = client
    .UsingBase()
    .WithRoute("api/sample")
    .WithJsonContent(model, SampleModelJsonContext.Default);
```

**Available Overloads**

- `WithJsonContent<T>(T, JsonTypeInfo<T>)`
- `WithJsonContent<T>(T, JsonTypeInfo<T>, string)`
- `WithJsonContent<T>(T, JsonTypeInfo<T>, MediaTypeHeaderValue)`
- `WithJsonContent<T>(T, JsonSerializerContext)`
- `WithJsonContent<T>(T, JsonSerializerContext, string)`
- `WithJsonContent<T>(T, JsonSerializerContext, MediaTypeHeaderValue)`

## Deserialize Typed JSON

The AOT-safe deserialization extensions use `JsonTypeInfo<T>` or `JsonSerializerContext`. All extension methods are applicable to `HttpResponseMessage` and `Task<HttpResponseMessage>`.

```csharp
// using JsonTypeInfo<T>
var model = await response.ReadJsonAsync(
    SampleModelJsonContext.Default.SampleModel,
    cancellationToken);

// using a JsonSerializerContext
var model = await response.ReadJsonAsync<SampleModel>(
    SampleModelJsonContext.Default,
    cancellationToken);
```

**Available Overloads**

- `ReadJsonAsync<T>(JsonTypeInfo<T>, CancellationToken)`
- `ReadJsonAsync<T>(JsonSerializerContext, CancellationToken)`

:::tip

If the response content is `null`, empty, or whitespace, these overloads return `default(T)` (typically `null` for reference types), matching the behavior of the reflection-based overloads.

:::

## Behavior Notes

All methods:

* Use `JsonTypeInfo<T>` or `JsonSerializerContext`.
* Do **not** require dynamic code generation.
* Return `default(T)` when the HTTP content is `null`, empty, or whitespace.
* When using `JsonSerializerContext` overloads, the context must include metadata for the generic type `T`. If it does not, an `InvalidOperationException` is thrown.
* The AOT-safe overloads do not use JsonSerializerOptions and cannot honor custom converters unless they are part of the source-generated context.

---

## Quick Reference

| Method                                                               | Description                                                                                                                         |
|----------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------|
| `WithJsonContent<T>(T, JsonTypeInfo<T>)`                             | Serializes a value to JSON using source-generated type metadata and sets it as request content with the default JSON media type.    |
| `WithJsonContent<T>(T, JsonTypeInfo<T>, string)`                     | Serializes a value using generated type metadata and applies the specified media type.                                              |
| `WithJsonContent<T>(T, JsonTypeInfo<T>, MediaTypeHeaderValue)`       | Serializes a value using generated type metadata and applies a fully configured content-type header.                                |
| `WithJsonContent<T>(T, JsonSerializerContext)`                       | Serializes a value using a serializer context (extracting its type metadata) and sets the default JSON media type.                  |
| `WithJsonContent<T>(T, JsonSerializerContext, string)`               | Serializes a value using a serializer context and applies the specified media type.                                                 |
| `WithJsonContent<T>(T, JsonSerializerContext, MediaTypeHeaderValue)` | Serializes a value using a serializer context and applies a fully configured content-type header.                                   |
| `ReadJsonAsync<T>(JsonTypeInfo<T>, CancellationToken)`               | Deserializes JSON from an HTTP response using generated type metadata, and returns `default(T) when content is empty or null.       |
| `ReadJsonAsync<T>(JsonSerializerContext, CancellationToken)`         | Deserializes JSON using a serializer context (extracting its type metadata), and returns `default(T) when content is empty or null. |
