---
sidebar_position: 4
title: Configure Content
---

FluentHttpClient provides a comprehensive set of extensions for attaching request content to an `HttpRequestBuilder`. These methods cover raw `HttpContent`, string content, JSON, XML, form-encoded payloads, and optional buffering.

## Adding `HttpContent`

Use this when you already have a concrete `HttpContent` instance.

```csharp
var multipart = new MultipartFormDataContent();
// ... add files, fields, etc.

var builder = client
    .UsingBase()
    .WithContent(multipart);
```

* Assigns the provided `HttpContent` directly.
* Ideal for multipart uploads, streams, or custom content types.

## String Content

These methods construct a `StringContent` payload with optional encoding and media type.

### Basic String

```csharp
builder.WithContent("raw text");
```

### With Encoding

```csharp
builder.WithContent("raw text", Encoding.UTF8);
```

### With Media Type

```csharp
builder.WithContent("raw text", "text/plain");
```

### With Encoding and Media Type

```csharp
builder.WithContent("raw text", Encoding.UTF8, "text/plain");
```

### With `MediaTypeHeaderValue`

```csharp
builder.WithContent("payload", new MediaTypeHeaderValue("application/custom"));
```

All overloads generate a `StringContent` instance and optionally set the `Content-Type` header.

## JSON Content

JSON content can be supplied as raw JSON strings or objects that will be serialized using `System.Text.Json`.

### Raw JSON strings

```csharp
builder.WithJsonContent("{ \"example\": true }");
```

Overloads support:

* Custom encoding
* Custom media type string or custom `MediaTypeHeaderValue`

### Typed JSON serialization

```csharp
builder.WithJsonContent(myObject);
```

You may also specify:

* Custom `JsonSerializerOptions`
* Custom media type string or custom `MediaTypeHeaderValue`

Example:

```csharp
var options = new JsonSerializerOptions { WriteIndented = true };
builder.WithJsonContent(myObject, options, "application/ld+json");
```

All JSON extensions serialize the object, create `StringContent`, and apply the relevant media type.

## XML Content

These methods attach XML content either from raw XML or using `System.Xml.Serialization` to serialize objects.

### Raw XML strings

```csharp
builder.WithXmlContent(xmlString);
```

Overloads allow:

* Custom encoding
* Custom media type string or custom `MediaTypeHeaderValue`

### Typed XML serialization

```csharp
builder.WithXmlContent(myObject);
```

You can refine XML serialization with:

* Custom `XmlWriterSettings`
* Custom media type string or custom `MediaTypeHeaderValue`

Example:

```csharp
var settings = new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 };
builder.WithXmlContent(myObject, settings, "application/custom+xml");
```

These extensions serialize the object to XML and set it as the request content. If you specify an Encoding in the XmlWriterSettings, that encoding is used both during XML serialization and as the encoding applied to the resulting StringContent.

## `FormUrlEncodedContent` Content

Produces standard `application/x-www-form-urlencoded` content.

### From Dictionary

```csharp
builder.WithFormContent(new Dictionary<string, string>
{
    ["username"] = "scott",
    ["password"] = "secret"
});
```

### From Key/Value Pairs

*Supports repeated keys.*

```csharp
var data = new[]
{
    new KeyValuePair<string, string>("role", "admin"),
    new KeyValuePair<string, string>("role", "editor")
};

builder.WithFormContent(data);
```

## Buffering Request Content

Some HTTP handlers or intermediaries require the request body to be fully in memory so `Content-Length` is known ahead of time.

### When buffering helps

* Servers that **reject chunked uploads**.
* **Request signing systems** requiring canonical byte sequences.
* **Retry logic** that must re-send identical content.
* Middleware that **reads or rewinds** the request body.

### When buffering hurts

* Large payloads dramatically increase memory usage.
* Not suitable for true streaming scenarios.
* Should be used only when required.

### Enable Buffering

```csharp
builder.WithBufferedContent();
```

Sets `BufferRequestContent = true` so the content is fully written into memory before sending.

## Behavior Notes

* Assigning content overrides any previously set content.
* JSON and XML extensions use UTF-8 by default.
* `MediaTypeHeaderValue` overloads bypass default media types.
* Streaming content may require buffering depending on the underlying handler.

---

## Quick Reference

| Method                     | Purpose                                           |
| -------------------------- | ------------------------------------------------- |
| `WithContent(HttpContent)` | Attach any existing `HttpContent` instance.       |
| `WithContent(string, ...)` | String content with optional encoding/media type. |
| `WithJsonContent(...)`     | Raw JSON or typed JSON serialization.             |
| `WithXmlContent(...)`      | Raw XML or typed XML serialization.               |
| `WithFormContent(...)`     | URL-encoded form data (dictionary or pairs).      |
| `WithBufferedContent()`    | Force buffering when required by handlers.        |
