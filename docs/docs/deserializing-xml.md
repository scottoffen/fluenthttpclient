---
sidebar_position: 13
title: Deserializing XML
---

FluentHttpClient provides a set of extensions for reading and deserializing XML from `HttpResponseMessage` instances and from tasks that produce them. These extensions support deserializing XML into concrete .NET types or parsing XML into `XElement` for flexible document-style access.

:::danger AOT and XML

For Native AOT builds, only the sections for `XElement` apply. FluentHttpClient does not have typed XML deserialization overloads that are AOT-friendly.

:::

## Typed Deserialization

Use these methods when you want to deserialize XML into a .NET reference type from either `HttpResponseMessage` or `Task<HttpResponseMessage>`.

```csharp
// In the fluent chain
var model = await client
    .UsingRoute("/api/data.xml")
    .GetAsync()
    .ReadXmlAsync<MyModel>();

// After awaiting the response
var model = await response.ReadXmlAsync<MyModel>();
```

**Available Overloads**

* `ReadXmlAsync<T>()`
* `ReadXmlAsync<T>(XmlReaderSettings)`
* `ReadXmlAsync<T>(CancellationToken)`
* `ReadXmlAsync<T>(XmlReaderSettings, CancellationToken)`

Empty or whitespace content returns `null`. Malformed XML throws the underlying serializer or XML parsing exception.

## XElement Parsing

Use these methods when you want to parse the response body into an `XElement` for LINQ-to-XML processing.

```csharp
// In the fluent chain
var element = await client
    .UsingRoute("/api/data.xml")
    .GetAsync()
    .ReadXmlElementAsync();

// After awaiting the response
var element = await response.ReadXmlElementAsync();
```

**Available Overloads**

* `ReadXmlElementAsync()`
* `ReadXmlElementAsync(LoadOptions)`
* `ReadXmlElementAsync(CancellationToken)`
* `ReadXmlElementAsync(LoadOptions, CancellationToken)`

Empty or whitespace input returns `null`. Malformed XML results in the underlying `XElement.Parse` exception.

## Behavior Notes

* Empty or whitespace content returns `null` for all XML deserialization and parsing operations.
* `CancellationToken` is honored in all methods that accept one.
* [`LoadOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.loadoptions) allow controlling whitespace, line information, and base URI preservation when parsing into `XElement`.
* See the documentation for [`XElement.Parse`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xelement.parse) for information on using `LoadOptions`.
---

## Quick Reference

| Method                                                  | Description                                                  |
| ------------------------------------------------------- | ------------------------------------------------------------ |
| `ReadXmlAsync<T>()`                                     | Deserializes XML into `T` using default serializer settings. |
| `ReadXmlAsync<T>(XmlReaderSettings)`                    | Uses custom XML reader settings.                             |
| `ReadXmlAsync<T>(CancellationToken)`                    | Observes a cancellation token.                               |
| `ReadXmlAsync<T>(XmlReaderSettings, CancellationToken)` | Full control over XML deserialization.                       |
| `ReadXmlElementAsync()`                                 | Parses XML into `XElement` using default options.            |
| `ReadXmlElementAsync(LoadOptions)`                      | Uses custom XML load options.                                |
| `ReadXmlElementAsync(CancellationToken)`                | Observes a cancellation token.                               |
| `ReadXmlElementAsync(LoadOptions, CancellationToken)`   | Full control over parsing.                                   |
