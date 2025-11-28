---
sidebar_position: 2
title: Configure Headers
---

FluentHttpClient lets you configure request headers directly on `HttpRequestBuilder`. Header configuration uses two approaches:

* **String headers** (`WithHeader`, `WithHeaders`) are validated and stored immediately when called, providing fast fail-fast validation for common header scenarios.
* **Typed headers** (`WithAuthentication`, `WithBasicAuthentication`, `WithOAuthBearerToken`, `ConfigureHeaders`) use deferred configurators that are applied when the `HttpRequestMessage` is built, allowing strongly-typed header configuration with complex types like `CacheControl` and `Authorization`.

## Adding Single Headers

Use `WithHeader` when you need to add one header to the request.

### Single Value

```csharp
var builder = client
    .UsingBase()
    .WithHeader("Accept", "application/json");
```

* Adds a header with one value.
* Throws if `key` or `value` is `null`.

### Multiple Values

```csharp
var builder = client
    .UsingBase()
    .WithHeader("X-Trace-Id", new[] { traceId, fallbackTraceId });
```

* Adds a header with multiple values for the same key.
* Throws if `key` or `values` is `null`.
* `values` must not be `null`, but may be an empty sequence.

## Adding Multiple Headers

When you need to add a batch of headers, use one of the `WithHeaders` overloads.

### Single Value per Header

```csharp
var headers = new[]
{
    new KeyValuePair<string, string>("Accept", "application/json"),
    new KeyValuePair<string, string>("X-Correlation-Id", correlationId)
};

var builder = client
    .UsingBase()
    .WithHeaders(headers);
```

* Adds multiple headers, one value per key.
* Throws `ArgumentException` if any header key is `null`.
* Throws `ArgumentException` if any header value is `null`.

### Multiple Values per Header

```csharp
var headers = new[]
{
    new KeyValuePair<string, IEnumerable<string>>(
        "Accept",
        new[] { "application/json", "application/problem+json" }),
    new KeyValuePair<string, IEnumerable<string>>(
        "X-Feature-Flag",
        new[] { "beta", "canary" })
};

var builder = client
    .UsingBase()
    .WithHeaders(headers);
```

* Adds multiple headers, each with (potentially) multiple values.
* Throws `ArgumentException` if any header key is `null`.
* Throws `ArgumentException` if any header value sequence is `null`.
* Header value sequence may be empty.

:::tip

Use the bulk overloads when you already have headers in a collection (e.g. from configuration or a shared helper).

:::

## Typed Headers

For headers that require strongly-typed values (such as `Authorization`, `CacheControl`, `Accept`, `IfModifiedSince`, `IfNoneMatch`, and others), use `ConfigureHeaders` to configure them directly through the `HttpRequestHeaders` API.

### Using ConfigureHeaders

```csharp
var builder = client
    .UsingBase()
    .ConfigureHeaders(headers =>
    {
        headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    });
```

* Provides direct access to the `HttpRequestHeaders` collection.
* Supports all strongly-typed header properties available on `HttpRequestHeaders`.
* Throws if `configure` action is `null`.
* Configuration is applied when the request is built (deferred execution).

### Complex Headers

`ConfigureHeaders` is ideal for headers with multiple properties, quality values, or complex structures:

#### Cache Control

```csharp
builder.ConfigureHeaders(headers =>
{
    headers.CacheControl = new CacheControlHeaderValue
    {
        NoCache = true,
        NoStore = true,
        MaxAge = TimeSpan.FromSeconds(30)
    };
});
```

#### Accept with Quality Values

```csharp
builder.ConfigureHeaders(headers =>
{
    headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
    headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain", 0.8));
});
```

#### Conditional Request Headers

```csharp
builder.ConfigureHeaders(headers =>
{
    headers.IfModifiedSince = lastModified;
    headers.IfNoneMatch.Add(new EntityTagHeaderValue("\"12345\""));
});
```

### Accumulation Behavior

Multiple calls to `ConfigureHeaders` accumulate - each configurator is stored and executed in order when the request is built:

```csharp
builder
    .ConfigureHeaders(headers =>
        headers.Authorization = new AuthenticationHeaderValue("Bearer", token))
    .ConfigureHeaders(headers =>
        headers.CacheControl = new CacheControlHeaderValue { NoCache = true })
    .ConfigureHeaders(headers =>
        headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")));

// All three configurators will be applied
```

If multiple configurators set the same header property, the last one wins (for single-value headers like `Authorization`). For collection-based headers (like `Accept`), all values accumulate unless a configurator explicitly clears the collection.

### When to Use ConfigureHeaders

Use `ConfigureHeaders` when you need:

* **Strongly-typed headers** like `Authorization`, `CacheControl`, `Accept`, `IfModifiedSince`
* **Headers with quality values** (e.g., `Accept: application/json;q=0.9`)
* **Headers with multiple properties** (e.g., `CacheControl` with NoCache, NoStore, MaxAge)
* **Collection-based headers** that support multiple values with typed entries
* **Direct access** to the `HttpRequestHeaders` API

For simple string-based headers (like `X-Correlation-Id` or `X-Tenant`), prefer `WithHeader` instead - it validates immediately, performs better, and keeps your code simpler.

### Performance Notes

`ConfigureHeaders` uses deferred execution, which means:

* Configurators are stored in a list and executed when the request is built.
* Multiple configurators have a small overhead compared to a single call.
* For high-throughput scenarios with simple headers, prefer `WithHeader`.

However, for most applications, the performance difference is negligible, and the strongly-typed API provides better compile-time safety and IntelliSense support.

## Reserved Headers

FluentHttpClient intentionally restricts a small set of HTTP headers that are controlled by the underlying `HttpClient` and its transport layers. These headers define wire-level framing and routing behavior, and overriding them can produce ambiguous requests, protocol violations, or security issues.

Because of this, the fluent string-header extensions (`WithHeader` and `WithHeaders`) do **not** allow setting the following headers:

* `Host`
* `Content-Length`
* `Transfer-Encoding`

These values are determined automatically based on the request URI, the configured `HttpContent`, and the negotiated HTTP version. Preventing them from being set through the fluent API helps avoid accidental misuse - such as combining `Content-Length` with `Transfer-Encoding: chunked` - while keeping request construction predictable.

### Advanced Usage

This restriction only applies to the `WithHeader` and `Withheaders` fluent extensions. If advanced scenarios require manual control of these headers, you can still accomplish this using `ConfigureHeaders`. This opt-in approach allows experienced users to take full control without exposing casual users to common footguns.

In short, the fluent API keeps the simple path safe, while still leaving the door open for expert customization or tom-foolery when needed.

:::tip Indirect Control

When you need indirect control over `Content-Length` or chunked transfer behavior, your lever is [`WithBufferedContent`](./configure-content.md#buffering-request-content). Buffered content *usually* produces a `Content-Length` header, while unbuffered or unknown-length content lets the runtime fall back to chunked transfer for HTTP/1.1. Nevertheless, FluentHttpClient itself **never sets these headers explicitly**.

:::

## Authentication Headers

For authentication, FluentHttpClient provides dedicated extensions that set the `Authorization` header.

### Arbitrary Scheme

```csharp
var builder = client
    .UsingBase()
    .WithAuthentication("CustomScheme", token);
```

* Sets `Authorization` to the given scheme and token.
* Throws if `scheme` or `token` is `null`.

### Basic Authentication

#### Using a pre-built token

```csharp
var builder = client
    .UsingBase()
    .WithBasicAuthentication(base64Token);
```

* Sets `Authorization: Basic {token}`.
* Assumes `token` is already a Base64-encoded `username:password` string.

#### Using Username and Password

```csharp
var builder = client
    .UsingBase()
    .WithBasicAuthentication("username", "password");
```

* Concatenates `username:password`.
* Encodes with UTF-8 and Base64.
* Sets `Authorization: Basic {encoded}`.

:::danger Security Notification

Basic auth is only Base64 encoding, not encryption. Avoid sending it over insecure connections and avoid logging the header or the raw password.

:::

### Bearer (OAuth) Tokens

```csharp
var builder = client
    .UsingBase()
    .WithOAuthBearerToken(accessToken);
```

* Sets `Authorization: Bearer {token}`.
* Throws if `token` is `null`.

## Behavior Notes

### String Header Methods

`WithHeader` and `WithHeaders` methods:

* Store headers in an internal dictionary with **immediate validation**.
* Validate header keys and values when the method is called (fail-fast).
* Reject reserved headers (`Host`, `Content-Length`, `Transfer-Encoding`) immediately.
* Headers are case-insensitive (per HTTP specification).
* Multiple values for the same header key are supported and accumulated.
* Headers are applied to the `HttpRequestMessage` when the request is built.

### Typed Header Methods

`WithAuthentication`, `WithBasicAuthentication`, `WithOAuthBearerToken`, and `ConfigureHeaders`:

* Add deferred configurators to `HttpRequestBuilder.HeaderConfigurators`.
* Validation occurs when the request is built (when you call `SendAsync`).
* Multiple configurators are cumulative and applied in order.
* The last configurator that sets a particular header wins (e.g., `Authorization`).
* Configurators have direct access to strongly-typed header properties.

### General Behavior

* All header configuration is cumulative - multiple calls add or update headers.
* String headers and typed headers can be mixed in the same request.
* Headers set via `WithHeader` take effect before typed header configurators run.
* Reserved headers (`Host`, `Content-Length`, `Transfer-Encoding`) cannot be set via string methods but may be set via advanced techniques (see Reserved Headers section).

---

## Quick Reference

| Method                                                                        | Purpose                                         |
| ----------------------------------------------------------------------------- | ----------------------------------------------- |
| `WithHeader(string key, string value)`                                        | Add a single header value.                      |
| `WithHeader(string key, IEnumerable<string> values)`                          | Add a header with multiple values.              |
| `WithHeaders(IEnumerable<KeyValuePair<string, string>> headers)`              | Add multiple headers, one value each.           |
| `WithHeaders(IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)` | Add multiple headers with multiple values each. |
| `ConfigureHeaders(Action<HttpRequestHeaders> configure)`                      | Configure strongly-typed headers directly.      |
| `WithAuthentication(string scheme, string token)`                             | Set `Authorization` with a custom scheme.       |
| `WithBasicAuthentication(string token)`                                       | Set `Authorization: Basic {token}`.             |
| `WithBasicAuthentication(string username, string password)`                   | Build and set a Basic auth token.               |
| `WithOAuthBearerToken(string token)`                                          | Set `Authorization: Bearer {token}`.            |
