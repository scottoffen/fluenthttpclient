---
sidebar_position: 11
title: Read Response Content
---

FluentHttpClient provides a set of extensions for reading the body of an `HttpResponseMessage` as a **string**, **stream**, or **byte array** that work both on a raw `HttpResponseMessage` and on `Task<HttpResponseMessage>` returned by fluent send operations.

These methods are intended for callers who want direct access to the raw response content. They are **different from the fluent deserialization** (JSON/XML).


:::important

In many cases, `HttpClient` buffers response content, especially when using the default `HttpCompletionOption.ResponseContentRead` behavior. However, buffering is not guaranteed, and when the response is streamed (such as when using `HttpCompletionOption.ResponseHeadersRead`), the content can only be read once.

If you plan to deserialize the content using `ReadJsonAsync` or `ReadXmlAsync`, you should probably avoid calling these content readers first.

Oh, and the cancellation tokens here apply **only to the read operation**, not to the original HTTP request.

:::

## Reading Content As A String

```csharp
// In the fluent chain
var text = await builder.GetAsync()
    .ReadContentAsStringAsync();

// After awaiting the response
var text = await response.ReadContentAsStringAsync();
```

**Available overloads**

- `Task<string> ReadContentAsStringAsync(this HttpResponseMessage responseMessage)`
- `Task<string> ReadContentAsStringAsync(this HttpResponseMessage responseMessage, CancellationToken cancellationToken)`
- `Task<string> ReadContentAsStringAsync(this Task<HttpResponseMessage> responseTask)`
- `Task<string> ReadContentAsStringAsync(this Task<HttpResponseMessage> responseTask, CancellationToken cancellationToken)`

These overloads wait for the response task, then read the content, and return `string.Empty` when content is null.

## Reading Content As A Stream

```csharp
// In the fluent chain
await using var stream = await builder.GetAsync()
    .ReadContentAsStreamAsync();

// After awaiting the response
await using var stream = await response.ReadContentAsStreamAsync();
```

**Available overloads**

- `Task<Stream> ReadContentAsStreamAsync(this HttpResponseMessage responseMessage)`
- `Task<Stream> ReadContentAsStreamAsync(this HttpResponseMessage responseMessage, CancellationToken cancellationToken)`
- `Task<Stream> ReadContentAsStreamAsync(this Task<HttpResponseMessage> responseTask)`
- `Task<Stream> ReadContentAsStreamAsync(this Task<HttpResponseMessage> responseTask, CancellationToken cancellationToken)`

These overloads wait for the response task, then return the content as a stream. On older platform, may return `Stream.Null` when the content is null.

## Reading Content As A Byte Array

```csharp
// in the fluent chain
var bytes = await builder.GetAsync()
    .ReadContentAsByteArrayAsync();

// After awaiting the response
var bytes = await response.ReadContentAsByteArrayAsync();
```

**Available overloads**

- `Task<byte[]> ReadContentAsByteArrayAsync(this HttpResponseMessage responseMessage)`
- `Task<byte[]> ReadContentAsByteArrayAsync(this HttpResponseMessage responseMessage, CancellationToken cancellationToken)`
- `Task<byte[]> ReadContentAsByteArrayAsync(this Task<HttpResponseMessage> responseTask)`
- `Task<byte[]> ReadContentAsByteArrayAsync(this Task<HttpResponseMessage> responseTask, CancellationToken cancellationToken)`

These overloads wait for the response task, then return the content as a byte array or an empty array when content is null. This is useful when you need to preserve content for multiple reads.

---

## Quick Reference

| Category      | Methods                                                                           |
| ------------- | --------------------------------------------------------------------------------- |
| String        | `ReadContentAsStringAsync()`, `ReadContentAsStringAsync(CancellationToken)`       |
| Stream        | `ReadContentAsStreamAsync()`, `ReadContentAsStreamAsync(CancellationToken)`       |
| Bytes         | `ReadContentAsByteArrayAsync()`, `ReadContentAsByteArrayAsync(CancellationToken)` |

These methods are ideal when you need the raw response content. For typed deserialization (JSON or XML), see the FluentHttpClient deserialization extensions.
