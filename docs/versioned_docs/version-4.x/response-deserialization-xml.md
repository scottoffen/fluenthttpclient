---
sidebar_position: 7
title: Response Deserialization - XML
---

FluentHttpClient includes built-in extension methods to deserialize HTTP responses into XML representations. These helpers make it easy to work with [`XElement`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xelement) or strongly typed objects using [`XmlSerializer`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.serialization.xmlserializer), while keeping your code clean and fluent.

## Overview

The XML deserialization extensions can convert the content of an `HttpResponseMessage` (or a `Task<HttpResponseMessage>`) into an `XElement` for flexible XML traversal, or into a strongly typed object for direct binding.

All methods are asynchronous and integrate seamlessly with other FluentHttpClient response operations.

## Deserialize to XElement

To get an `XElement` representation of the response, call one of the `DeserializeXmlAsync()` overloads.

```csharp
var xml = await client
    .UsingRoute("/users/123")
    .GetAsync()
    .DeserializeXmlAsync();
```

This reads the response content stream and builds a new `XElement` representing the full XML document.

### Preserving whitespace and line info

You can optionally specify `LoadOptions` to control how whitespace and line information are handled:

```csharp
var xml = await client
    .UsingRoute("/users/123")
    .GetAsync()
    .DeserializeXmlAsync(LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
```

These flags mirror the options available in LINQ to XML’s `XElement.LoadAsync()` method.

## Deserialize to a Strongly Typed Object

You can also deserialize the response directly into a class using the built-in `XmlSerializer`.

```csharp
public class User
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
}

var user = await client
    .UsingRoute("/users/123")
    .GetAsync()
    .DeserializeXmlAsync<User>();
```

This method automatically reads the content stream, constructs an `XmlSerializer` for `User`, and returns the deserialized instance.

If the response is empty, invalid XML, or not compatible with the target type, an `InvalidOperationException` will be thrown by the underlying serializer.

## Cancellation Support

All XML deserialization methods support a `CancellationToken` parameter for improved control over long-running requests:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

var xml = await client
    .UsingRoute("/slow-feed")
    .GetAsync()
    .DeserializeXmlAsync(LoadOptions.None, cts.Token);
```

Cancelling the token before or during deserialization will stop the operation and throw a `TaskCanceledException`.

## Using With HttpResponseMessage

You can also call `DeserializeXmlAsync()` directly on an `HttpResponseMessage` if you already have one:

```csharp
var response = await client.UsingRoute("/feed").GetAsync();
var xml = await response.DeserializeXmlAsync();
```

This is especially useful when you want to inspect the response headers or status code before parsing the content.

## Behavior and Stream Handling

* The underlying response stream is read **to completion** during deserialization.
* Once deserialization is complete, the stream cannot be re-read unless it is buffered or reassigned.
* For most use cases, you should call deserialization methods only once per response.

If you need to inspect or log the content before deserialization, use `GetResponseStringAsync()` or `GetResponseStreamAsync()` first, buffer the content, and then deserialize from that stream.

## Feature Differences Compared to JSON Deserialization

While XML deserialization offers the same core functionality, not all features available in JSON deserialization are currently supported.

* XML deserialization does **not** include fallback delegates (e.g., `Func<HttpResponseMessage, Exception, T>`) for error handling.
* JSON deserialization allows custom `JsonSerializerOptions`; XML deserialization uses `XmlSerializer` and standard `XmlReader`/`XElement` configuration instead.
* Error handling is less granular - exceptions from `XmlSerializer` or malformed XML are thrown directly.
* Future releases will bring feature parity for fallback handling and enhanced serializer configuration.

These differences are intentional for now to prioritize performance and simplicity. Full parity is planned for a future major version.

## Example: Deserialize Atom or RSS Feeds

Because these methods use `XElement` under the hood, they work great for feed parsing and custom XML payloads.

```csharp
var feed = await client
    .UsingRoute("/rss")
    .GetAsync()
    .DeserializeXmlAsync();

foreach (var item in feed.Elements("channel").Elements("item"))
{
    Console.WriteLine(item.Element("title")?.Value);
}
```

## Summary

| Use Case                         | Recommended Method                            | Return Type       |
| -------------------------------- | --------------------------------------------- | ----------------- |
| Read XML as an `XElement`        | `DeserializeXmlAsync()`                       | `XElement`        |
| Read XML as a typed object       | `DeserializeXmlAsync<T>()`                    | `T?`              |
| Preserve whitespace or line info | `DeserializeXmlAsync(LoadOptions)`            | `XElement`        |
| Support cancellation             | `DeserializeXmlAsync(..., CancellationToken)` | `XElement` / `T?` |

All XML deserialization methods are built on modern async APIs (`XElement.LoadAsync` and `XmlSerializer`), making them efficient, composable, and compatible with .NET 6.0 and newer.
