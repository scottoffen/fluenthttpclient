---
sidebar_position: 12
title: Deserializing JSON
---

FluentHttpClient provides a set of extensions for reading and deserializing JSON from `HttpResponseMessage` instances and from tasks that produce them. These extensions support strongly typed models, `JsonDocument`, and - on .NET 6 and later - `JsonObject` from `System.Text.Json.Nodes`.

## Typed Deserialization

Use these methods when you want to deserialize JSON into a .NET type from `Task<HttpResponseMessage>` or `HttpResponseMessage`.

```csharp
// In the fluent chain
var model = await client
    .UsingRoute("/api/data")
    .GetAsync()
    .ReadJsonAsync<MyModel>();

// After awaiting the response
var model = await response.ReadJsonAsync<MyModel>();
```

**Available Overloads**

- `ReadJsonAsync<T>()`
- `ReadJsonAsync<T>(JsonSerializerOptions?)`
- `ReadJsonAsync<T>(CancellationToken)`
- `ReadJsonAsync<T>(JsonSerializerOptions?, CancellationToken)`


## JsonDocument Parsing

`JsonDocument` provides an efficient, read-only view of JSON payloads with lower overhead than `JsonNode`-based APIs, making it a good fit for scenarios where you need structured access without mutating the data. Use these methods when you want to deserialize JSON into a `JsonDocument` object from `Task<HttpResponseMessage>` or `HttpResponseMessage`.

```csharp
// In the fluent chain
var doc = await client
    .UsingRoute("/api/data")
    .GetAsync()
    .ReadJsonDocumentAsync();

//After awaiting the response
using var doc = await response.ReadJsonDocumentAsync();
```

**Available Overloads**

- `ReadJsonDocumentAsync()`
- `ReadJsonDocumentAsync(JsonDocumentOptions)`
- `ReadJsonDocumentAsync(CancellationToken)`
- `ReadJsonDocumentAsync(JsonDocumentOptions, CancellationToken)`

## JsonObject Parsing

`JsonObject` is mutable and ideal for lightweight manipulation of dynamic or semi-structured JSON. Use these methods when you want to deserialize JSON into a `JsonObject` object from `Task<HttpResponseMessage>` or `HttpResponseMessage`.

:::important JsonObject Support

These methods are only available on .NET 6 and later, as they rely on `JsonObject`, which is only available in .NET 6.0 and higher as part of the `System.Text.Json.Nodes` API.

:::

```csharp
// In the fluent chain
var obj = await client
    .UsingRoute("/api/data")
    .GetAsync()
    .ReadJsonObjectAsync();

// After awaiting the response
var obj = await response.ReadJsonObjectAsync();
```

**Available Overloads**
- `ReadJsonObjectAsync()`
- `ReadJsonObjectAsync(CancellationToken)`
- `ReadJsonObjectAsync(JsonNodeOptions)`
- `ReadJsonObjectAsync(JsonNodeOptions, CancellationToken)`
- `ReadJsonObjectAsync(JsonDocumentOptions)`
- `ReadJsonObjectAsync(JsonDocumentOptions, CancellationToken)`
- `ReadJsonObjectAsync(JsonNodeOptions, JsonDocumentOptions)`
- `ReadJsonObjectAsync(JsonNodeOptions, JsonDocumentOptions, CancellationToken)`

## Behavior Notes

* While `HttpResponseMessage.Content` is rarely null on modern TFMs, if the response has no content (`Content` is `null`), then all methods return `default`/`null` instead of throwing.
* All deserialization is performed using `System.Text.Json`.
* Stream reading honors `CancellationToken` on TFMs that support it.
* `JsonObject` APIs are only available when targeting `.NET 6` or later.

---

## Quick Reference

| Method                                                                         | Description                                      |
| -------------------------------------------------------------------------------| ------------------------------------------------ |
| `ReadJsonAsync<T>()`                                                           | Deserializes using default serializer options.   |
| `ReadJsonAsync<T>(JsonSerializerOptions?)`                                     | Uses the provided serializer options.            |
| `ReadJsonAsync<T>(CancellationToken)`                                          | Observes a cancellation token.                   |
| `ReadJsonAsync<T>(JsonSerializerOptions?, CancellationToken)`                  | Full control over options and cancellation.      |
| `ReadJsonDocumentAsync()`                                                      | Parses JSON using default `JsonDocumentOptions`. |
| `ReadJsonDocumentAsync(JsonDocumentOptions)`                                   | Uses custom document options.                    |
| `ReadJsonDocumentAsync(CancellationToken)`                                     | Observes a cancellation token.                   |
| `ReadJsonDocumentAsync(JsonDocumentOptions, CancellationToken)`                | Full control over parsing.                       |
| `ReadJsonObjectAsync()`                                                        | Uses default node and document options.          |
| `ReadJsonObjectAsync(CancellationToken)`                                       | Adds cancellation support.                       |
| `ReadJsonObjectAsync(JsonNodeOptions)`                                         | Customizes node behavior.                        |
| `ReadJsonObjectAsync(JsonNodeOptions, CancellationToken)`                      | Node options + cancellation.                     |
| `ReadJsonObjectAsync(JsonDocumentOptions)`                                     | Customizes parsing behavior.                     |
| `ReadJsonObjectAsync(JsonDocumentOptions, CancellationToken)`                  | Document options + cancellation.                 |
| `ReadJsonObjectAsync(JsonNodeOptions, JsonDocumentOptions)`                    | Combines both option types.                      |
| `ReadJsonObjectAsync(JsonNodeOptions, JsonDocumentOptions, CancellationToken)` | Full control.                                    |
