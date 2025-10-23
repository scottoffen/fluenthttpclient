---
sidebar_position: 2
title: HttpRequestBuilder
---

The `HttpRequestBuilder` class provides a fluent, flexible way to construct and send HTTP requests using an existing `HttpClient` instance. It follows the builder pattern to let you configure headers, cookies, content, and other options before sending the request.

None of the properties is required as long as the provided `HttpClient` instance knows where to send the request.

:::important Recommendation
Wherever possible, prefer configuring properties and calling methods through the **FluentHttpClient** extension methods, rather than modifying `HttpRequestBuilder` directly.
:::

## Creating a Request

You can create a new `HttpRequestBuilder` by passing an `HttpClient` instance. An optional route can also be provided to target a specific endpoint.

```csharp
var builder = new HttpRequestBuilder(client);
var builder = new HttpRequestBuilder(client, "/users/12345");
```

While you *can* create the builder directly, it is recommended to use the extension methods on `HttpClient`.

```csharp
var request = client.UsingRoute("/users/12345");
var request = client.WithoutRoute();
```

When the request is sent:
- If the `HttpClient.BaseAddress` is set, the route is appended to it.
- If not, the route becomes the full request URL.

---

## Properties

### Content

Specifies the `HttpContent` to send with the request (e.g., JSON, form data, or multipart content).

```csharp
builder.Content = new StringContent(json, Encoding.UTF8, "application/json");
```

See:
- `WithContent(HttpContent content)`
- `WithContent(string content)`
- `WithContent(string content, string contentType)`
- `WithContent(string content, MediaTypeHeaderValue contentTypeHeaderValue)`
- `WithJsonContent<T>(T content)`
- `WithJsonContent<T>(T content, JsonSerializerOptions options)`
- `WithXmlContent<T>(T obj)`
- `WithXmlContent<T>(T obj, Encoding encoding)`
- `WithXmlContent<T>(T obj, string contentType)`
- `WithXmlContent<T>(T obj, Encoding encoding, string contentType)`
- `WithXmlContent(string xml)`
- `WithXmlContent(string xml, Encoding encoding)`
- `WithXmlContent(string xml, string contentType)`
- `WithXmlContent(string xml, Encoding encoding, string contentType)`

### Cookies

Manages cookies to include with the request via a [`CookieContainer`](https://learn.microsoft.com/en-us/dotnet/api/system.net.cookiecontainer).

```csharp
builder.Cookies.Add(new Uri("https://api.example.com"), new Cookie("session", "abc123"));
```

See:
- `WithCookie(Cookie cookie)`
- `WithCookie(CookieCollection cookieCollection)`

### ConfigureHeaderActions

A collection of actions that can modify request headers before sending.

```csharp
builder.ConfigureHeaderActions.Add(headers => headers.Add("X-Custom-Header", "value"));
```

See:
- `WithHeader(string key, string value)`
- `WithHeader(string key, IEnumerable<string> values)`
- `WithHeaders(IEnumerable<KeyValuePair<string, string>> headers)`
- `WithAuthentication(string scheme, string token)`
- `WithBasicAuthentication(string token)`
- `WithBasicAuthentication(string username, string password)`
- `WithOAuthBearerToken(string token)`

### ConfigureOptionsAction

An optional action for configuring [`HttpRequestOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestoptions).

```csharp
builder.ConfigureOptionsAction = options =>
{
    options.Set("RetryCount", 3);
};
```

See:
- `ConfigureOptions(Action<HttpRequestOptions> action)`

### BufferContentBeforeSending

A boolean value that indicates whether content should be buffered before sending.

See:
- `WithPreloadedContent()`

### QueryParams

A list of query parameters to append to the request URI.

See:
- `WithQueryParam(string key, string? value)`
- `WithQueryParam(string key, object? value)`
- `WithQueryParams(IEnumerable<KeyValuePair<string, string?>> values)`
- `WithQueryParams(IEnumerable<KeyValuePair<string, object?>> values)`
- `WithQueryParams(NameValueCollection values)`
- `WithQueryParamIfNotNull(string key, string? value)`
- `WithQueryParamIfNotNull(string key, object? value)`

### Route

Defines the request route, appended to `BaseAddress` when sending.

```csharp
builder.Route = "/users/12345";
```

See:
- `UsingRoute(string route)`
- `WithoutRoute()`

### Version and VersionPolicy

Controls the HTTP version and negotiation policy for the request.

```csharp
builder.Version = new Version("2.0");
builder.VersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
```

See:
- `UsingVersion(string version)`
- `UsingVersion(Version version)`
- `UsingVersion(string version, HttpVersionPolicy policy)`
- `UsingVersion(Version version, HttpVersionPolicy policy)`

## Sending Requests

You can send a request using one of the `SendAsync` overloads. All of them take an `HttpMethod` as the first parameter. [`HttpCompleteOption`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpcompletionoption) flags and a [`CancellationToken`](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken) are optional parameters.

```csharp
var response = await builder.SendAsync(
    HttpMethod.Post,
    HttpCompletionOption.ResponseContentRead,
    CancellationToken.None
);
```

See:
- `DeleteAsync()`
- `GetAsync()`
- `PostAsync()`
- `PutAsync()`

### Error Handling

`SendAsync` can throw several exceptions.

| Exception                   | Reason                                |
|-----------------------------|---------------------------------------|
| `ArgumentNullException`     | The route or request is invalid.      |
| `InvalidOperationException` | The request message was already sent. |
| `HttpRequestException`      | For network, DNS, or SSL errors.      |
| `TaskCanceledException`     | The request timed out.                |

---

## Design Notes

### When to Buffer Content Before Sending

Setting `BufferContentBeforeSending` controls whether the request content is pre-buffered (fully serialized into memory) before the HTTP request is sent.

**When `true`:**
The entire request body (e.g., JSON, multipart form data, file uploads) is serialized and loaded into memory before transmission begins. This ensures that the `Content-Length` header can be accurately set and that retries or redirects can reuse the buffered content. However, it can significantly increase memory usage, especially for large payloads.

**When `false` (default):**
The content is streamed directly to the network without being fully buffered first. This reduces memory pressure and allows for more efficient transmission of large or streaming payloads, but may prevent the `Content-Length` header from being calculated ahead of time.
Some edge cases - such as certain authentication flows or proxies - might fail when the length isn't known in advance.

**Recommendation:**
Keep this value false unless you are encountering issues with streaming content or need to ensure the full payload is buffered for retry, redirect, or compatibility scenarios (e.g., [dotnet/runtime#30283](https://github.com/dotnet/runtime/issues/30283)).

### Why Headers Use a Collection of Actions

Headers in `HttpRequestBuilder` are implemented as a **collection of actions** (`List<Action<HttpRequestHeaders>>`) instead of a direct collection of header values.
This design provides several key advantages:

1. **Deferred Execution**
   Each action is executed *when the request is being built*, ensuring that headers are applied to the actual `HttpRequestMessage` instance just before sending.
   This avoids premature binding and ensures headers are always applied in the correct context.

2. **Access to Full `HttpRequestHeaders` API**
   By passing the `HttpRequestHeaders` object into each action, callers can use the full native API (e.g., `Add`, `TryAddWithoutValidation`, `UserAgent.Add`, etc.) without being limited to simple key-value pairs.

3. **Support for Fluent Composition**
   Fluent extension methods (like `WithHeader()`, `WithAuthorization()`, or `WithJsonContent()`) can easily append new configuration actions without interfering with existing ones.
   This allows multiple methods to independently contribute to the final request headers.

4. **No Premature Materialization**
   Because actions are stored rather than concrete headers, there's no need to maintain a mutable intermediate header dictionary or merge duplicates.
   Headers are added directly to the outgoing request at send time, ensuring correct behavior even if the same builder is reused with different configurations.

While this adds complexity when working directly with the `ConfigureHeaderActions` property, it is recommended instead to use the extension methods provided, which abstracts away this complexity.

---

## Summary

`HttpRequestBuilder` provides the foundation of **FluentHttpClient**'s fluent API. It handles low-level details such as combining routes, applying headers, managing cookies, and sending requests. In most cases, you'll interact with it through fluent methods like `UsingRoute()`, `WithHeader()`, and `WithJsonContent()` for a cleaner, more expressive workflow.
