---
sidebar_position: 10
title: Sending Requests
---

Once you have configured an `HttpRequestBuilder` with a route, headers, query string, content, and any conditional behaviors, you eventually need to send the request. FluentHttpClient provides method-specific send extensions that mirror the familiar `HttpClient` API while preserving the fluent builder experience.

## Method Specific Extensions

For each common HTTP verb, FluentHttpClient provides a family of `*Async` methods that send the request using that verb.

Each method:

* Uses the configured builder state to construct an `HttpRequestMessage`.
* Applies any deferred configurators, headers, cookies, and options.
* Delegates to the underlying `HttpClient.SendAsync` with the selected `HttpMethod`, `HttpCompletionOption`, and `CancellationToken`.

### GET

Use GET for retrieving resources without a request body.

```csharp
var response = await builder.GetAsync();
```

**Available overloads**

* `Task<HttpResponseMessage> GetAsync()`
* `Task<HttpResponseMessage> GetAsync(CancellationToken cancellationToken)`
* `Task<HttpResponseMessage> GetAsync(HttpCompletionOption completionOption)`
* `Task<HttpResponseMessage> GetAsync(HttpCompletionOption completionOption, CancellationToken cancellationToken)`

### POST

Use POST for creating resources or sending commands, typically with a request body.

```csharp
var response = await builder
    .WithJsonContent(requestModel)
    .PostAsync();
```

**Available overloads**

* `Task<HttpResponseMessage> PostAsync()`
* `Task<HttpResponseMessage> PostAsync(CancellationToken cancellationToken)`
* `Task<HttpResponseMessage> PostAsync(HttpCompletionOption completionOption)`
* `Task<HttpResponseMessage> PostAsync(HttpCompletionOption completionOption, CancellationToken cancellationToken)`

### PUT

Use PUT for idempotent updates or full resource replacement.

```csharp
var response = await builder
    .WithJsonContent(updateModel)
    .PutAsync();
```

**Available overloads**

* `Task<HttpResponseMessage> PutAsync()`
* `Task<HttpResponseMessage> PutAsync(CancellationToken cancellationToken)`
* `Task<HttpResponseMessage> PutAsync(HttpCompletionOption completionOption)`
* `Task<HttpResponseMessage> PutAsync(HttpCompletionOption completionOption, CancellationToken cancellationToken)`

### DELETE

Use DELETE for removing resources.

```csharp
var response = await builder.DeleteAsync();
```

**Available overloads**

* `Task<HttpResponseMessage> DeleteAsync()`
* `Task<HttpResponseMessage> DeleteAsync(CancellationToken cancellationToken)`
* `Task<HttpResponseMessage> DeleteAsync(HttpCompletionOption completionOption)`
* `Task<HttpResponseMessage> DeleteAsync(HttpCompletionOption completionOption, CancellationToken cancellationToken)`

### HEAD

HEAD requests retrieve only headers (no response body), which can be useful for:

* Checking whether a resource exists.
* Inspecting metadata (e.g., content length, ETag) without downloading full content.

```csharp
var response = await builder.HeadAsync();
```

**Available overloads**

* `Task<HttpResponseMessage> HeadAsync()`
* `Task<HttpResponseMessage> HeadAsync(CancellationToken cancellationToken)`
* `Task<HttpResponseMessage> HeadAsync(HttpCompletionOption completionOption)`
* `Task<HttpResponseMessage> HeadAsync(HttpCompletionOption completionOption, CancellationToken cancellationToken)`

### OPTIONS

OPTIONS requests ask the server which HTTP methods and features are supported for a resource.

They can be used to:

* Inspect allowed methods.
* Probe CORS or other capabilities.

```csharp
var response = await builder.OptionsAsync();
```

**Available overloads**

* `Task<HttpResponseMessage> OptionsAsync()`
* `Task<HttpResponseMessage> OptionsAsync(CancellationToken cancellationToken)`
* `Task<HttpResponseMessage> OptionsAsync(HttpCompletionOption completionOption)`
* `Task<HttpResponseMessage> OptionsAsync(HttpCompletionOption completionOption, CancellationToken cancellationToken)`

### PATCH

PATCH is commonly used for partial updates.

```csharp
var response = await builder
    .WithJsonContent(patchDocument)
    .PatchAsync();
```

**Available overloads**

* `Task<HttpResponseMessage> PatchAsync()`
* `Task<HttpResponseMessage> PatchAsync(CancellationToken cancellationToken)`
* `Task<HttpResponseMessage> PatchAsync(HttpCompletionOption completionOption)`
* `Task<HttpResponseMessage> PatchAsync(HttpCompletionOption completionOption, CancellationToken cancellationToken)`

## Using Custom Methods

FluentHttpClient includes a set of core `SendAsync` methods on `HttpRequestBuilder` that allow you to send requests using *any* `HttpMethod` - including methods that do not have dedicated convenience extensions, such as `TRACE`, `LINK`, `UNLINK`, `PROPFIND`, or entirely custom methods.

These overloads are useful when:

* You need to call an API that uses uncommon HTTP verbs.
* You want to send requests using a custom `HttpMethod` instance.

```csharp
// Example using TRACE
var response = await builder.SendAsync(HttpMethod.Trace);

// Example using custom method (as a string)
var response = await builder.SendAsync("purge");
```

**Available overloads**

- `Task<HttpResponseMessage> SendAsync(HttpMethod method)`
- `Task<HttpResponseMessage> SendAsync(string method)`
- `Task<HttpResponseMessage> SendAsync(HttpMethod method, CancellationToken cancellationToken)`
- `Task<HttpResponseMessage> SendAsync(string method, CancellationToken cancellationToken)`
- `Task<HttpResponseMessage> SendAsync(HttpMethod method, HttpCompletionOption completionOption)`
- `Task<HttpResponseMessage> SendAsync(string method, HttpCompletionOption completionOption)`
- `Task<HttpResponseMessage> SendAsync(HttpMethod method, HttpCompletionOption completionOption, CancellationToken cancellationToken)`
- `Task<HttpResponseMessage> SendAsync(string method, HttpCompletionOption completionOption, CancellationToken cancellationToken)`

:::tip

When a string parameter is provided, the underlying method will automatically convert the string to upper case and use it to create a new `HttpMethod` instance.

In both cases, the `method` parameter must not be `null`; otherwise an `ArgumentNullException` is thrown.

:::

## Http Completion Options

All overloads that accept `HttpCompletionOption` control how much of the response is buffered before the `Task` completes.

* `HttpCompletionOption.ResponseContentRead` (default)

  * The `Task` only completes after the entire response body has been read.
  * Simplifies typical scenarios where you immediately read or deserialize the content.

* `HttpCompletionOption.ResponseHeadersRead`

  * The `Task` completes as soon as response headers are available.
  * The response body is streamed and can be read later.
  * Useful for large payloads or when you want more control over streaming.

Example:

```csharp
var response = await builder
    .GetAsync(HttpCompletionOption.ResponseHeadersRead, cancellationToken);

await using var stream = await response.ReadContentAsStreamAsync(cancellationToken);
// Process stream as it arrives
```

## Cancellation Tokens

Every verb helper has overloads that accept a `CancellationToken`.

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

var response = await builder.GetAsync(cts.Token);
```

Cancellation tokens are passed down to the underlying `HttpClient.SendAsync` call and:

* Allow callers to stop requests that take too long.
* Integrate with higher-level cancellation (e.g., ASP.NET Core request cancellation).

You can combine `HttpCompletionOption` and `CancellationToken` in a single call:

```csharp
var response = await builder
    .PostAsync(HttpCompletionOption.ResponseHeadersRead, cancellationToken);
```

## Behavior Notes

* A valid request URI is required - either from `HttpClient.BaseAddress` or `builder.Route`.
* All headers, cookies, query parameters, options, and deferred configurators are applied before sending.
* The request is built fresh each time - safe for reuse across multiple requests.

---

## Quick Reference

| Method group   | Overloads                                                                                                                                          |
| -------------- | -------------------------------------------------------------------------------------------------------------------------------------------------- |
| `GetAsync`     | `GetAsync()`, `GetAsync(CancellationToken)`, `GetAsync(HttpCompletionOption)`, `GetAsync(HttpCompletionOption, CancellationToken)`                 |
| `PostAsync`    | `PostAsync()`, `PostAsync(CancellationToken)`, `PostAsync(HttpCompletionOption)`, `PostAsync(HttpCompletionOption, CancellationToken)`             |
| `PutAsync`     | `PutAsync()`, `PutAsync(CancellationToken)`, `PutAsync(HttpCompletionOption)`, `PutAsync(HttpCompletionOption, CancellationToken)`                 |
| `DeleteAsync`  | `DeleteAsync()`, `DeleteAsync(CancellationToken)`, `DeleteAsync(HttpCompletionOption)`, `DeleteAsync(HttpCompletionOption, CancellationToken)`     |
| `HeadAsync`    | `HeadAsync()`, `HeadAsync(CancellationToken)`, `HeadAsync(HttpCompletionOption)`, `HeadAsync(HttpCompletionOption, CancellationToken)`             |
| `OptionsAsync` | `OptionsAsync()`, `OptionsAsync(CancellationToken)`, `OptionsAsync(HttpCompletionOption)`, `OptionsAsync(HttpCompletionOption, CancellationToken)` |
| `PatchAsync`   | `PatchAsync()`, `PatchAsync(CancellationToken)`, `PatchAsync(HttpCompletionOption)`, `PatchAsync(HttpCompletionOption, CancellationToken)`         |
| `SendAsync`    | `SendAsync(HttpMethod)`, `SendAsync(string)`, `SendAsync(HttpMethod, CancellationToken)`, `SendAsync(string, CancellationToken)`, `SendAsync(HttpMethod, HttpCompletionOption)`, `SendAsync(string, HttpCompletionOption)`, `SendAsync(HttpMethod, HttpCompletionOption, CancellationToken)`, `SendAsync(string, HttpCompletionOption, CancellationToken)` |

:::note

All method-specific overloads ultimately delegate to `HttpRequestBuilder.SendAsync`, which handles message construction and forwards the call to the underlying `HttpClient`.

:::