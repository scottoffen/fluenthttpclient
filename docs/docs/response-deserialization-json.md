---
sidebar_position: 7
title: Response Deserialization - JSON
---

FluentHttpClient makes it simple to convert JSON from an HTTP response into strongly typed objects. These extensions are built around `System.Text.Json` and integrate directly with the `HttpResponseMessage`, `Stream`, and `string` types.

This page focuses on **JSON deserialization**. XML deserialization is covered separately.

## Overview

When you call a FluentHttpClient request method (e.g., `GetAsync()` or `PostAsync()`), the response content is available as an `HttpResponseMessage`. Instead of manually reading and parsing JSON, you can use the provided deserialization extensions:

```csharp
var user = await client
    .UsingRoute("/users/123")
    .GetAsync()
    .DeserializeJsonAsync<User>();
```

FluentHttpClient automatically reads the response stream, deserializes the JSON payload into the specified type, and returns it.

If the response is empty or the JSON is invalid, a `JsonException` may be thrown.

## Deserializing from a Stream

If you have already read the response body into a stream, deserialization can be done from the `Stream` or `Task<Stream>`: 

```csharp
var user = await client
    .UsingRoute("/users/123")
    .GetAsync()
    .GetResponseStreamAsync()
    .DeserializeJson<User>();
```

## Deserializing from a String

If you have already read the response into a string, deserialization can be done from a `string` or `Task<string>`:

```csharp
var user = await client
    .UsingRoute("/users/123")
    .GetAsync()
    .GetResponseStringAsync()
    .DeserializeJson<User>();
```

## Error Handling

In some cases, you may want to define a fallback behavior when deserialization fails (for example, returning a default object or logging the error). You can pass a delegate that runs when an exception occurs.

The delegate provided for handling exceptions has two possible signatures.

- `Func<HttpResponseMessage, Exception, T>` — Handle errors using the response and exception.
- `Func<Exception, T>` — Handle errors with only the exception.
- Async variants are also available (`Func<..., Task<T>>`).

In each case, the delegate should return a valid instance of type `T`. This ensures your application can recover gracefully even if the response content isn't as expected.

```csharp
var user = await client
    .UsingRoute("/users/123")
    .GetAsync()
    .DeserializeJsonAsync<User>((response, ex) =>
    {
        Console.WriteLine($"Failed to deserialize: {ex.Message}");
        return new User { Name = "Unknown" };
    });

var user = await stream.DeserializeJsonAsync<User>(ex =>
{
    Console.WriteLine($"Error: {ex.Message}");
    return new User();
});
```

These options give you full control over how to handle invalid JSON, network interruptions, or unexpected content.

## Cancellation

All asynchronous deserialization methods support cancellation tokens and optional serializer configuration:

```csharp
var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
var options = new JsonSerializerOptions { WriteIndented = true };

var user = await client
    .UsingRoute("/users/123")
    .GetAsync()
    .DeserializeJsonAsync<User>(options, cts.Token);
```

This makes FluentHttpClient deserialization flexible for both production and high-performance use cases.

## Default JSON Options

Unless you pass explicit `JsonSerializerOptions`, FluentHttpClient uses a shared default:

```csharp
public static class FluentHttpClientOptions
{
    public static JsonSerializerOptions DefaultJsonSerializerOptions { get; set; } = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}
```

**What this means in practice**

* **Case-insensitive properties**: Incoming JSON can use any casing (e.g., `firstName`, `FirstName`) and still bind to your C# properties.
* **Flexible numbers**: Numeric values encoded as strings (e.g., `"42"`) are accepted during deserialization.
* **Null writing ignored**: When *serializing* (e.g., via `WithJsonContent`), `null` properties are omitted by default.

### Changing the defaults

You can override the defaults at application startup to affect all FluentHttpClient JSON (de)serialization that does **not** supply explicit options:

```csharp
// e.g., in Program.cs or composition root
FluentHttpClientOptions.DefaultJsonSerializerOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    Converters = { new JsonStringEnumConverter() }
};
```

:::danger Global Impact

This is a static setting. Changing it affects every call that relies on the defaults across your process. Prefer passing explicit options to a single call when you need per-request behavior.

:::

**Per-call override example**

```csharp
var customOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = false,
    Converters = { new JsonStringEnumConverter() }
};

var user = await client
    .UsingRoute("/users/123")
    .GetAsync()
    .DeserializeJsonAsync<User>(customOptions);
```

## Summary

FluentHttpClient's JSON deserialization extensions let you:

* Convert JSON directly from strings, streams, or HTTP responses.
* Customize serialization options and cancellation behavior.
* Provide fallback handlers for failed deserialization.
* Keep your code clean, readable, and free from repetitive boilerplate.

With these tools, turning API responses into typed objects becomes effortless and reliable.
