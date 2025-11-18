---
sidebar_position: 2
title: Configure Headers
---

FluentHttpClient lets you configure request headers directly on `HttpRequestBuilder`. All header methods add *deferred configurators* to the builder, so headers are applied when the `HttpRequestMessage` is finally built, not when you call the fluent methods.

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

All of these methods work by adding actions to `HttpRequestBuilder.HeaderConfigurators`:

* Headers are applied when the request is built (for example, when you call `SendAsync`).
* Multiple calls to header methods are cumulative:
  * Multiple non-auth headers are combined as expected.
  * The most recent method that sets `Authorization` wins.
* Headers are added via `TryAddWithoutValidation`, which:
  * Skips strict header format checks.
  * Still respects HTTP semantics at send time.

---

## Quick Reference

| Method                                                                        | Purpose                                         |
| ----------------------------------------------------------------------------- | ----------------------------------------------- |
| `WithHeader(string key, string value)`                                        | Add a single header value.                      |
| `WithHeader(string key, IEnumerable<string> values)`                          | Add a header with multiple values.              |
| `WithHeaders(IEnumerable<KeyValuePair<string, string>> headers)`              | Add multiple headers, one value each.           |
| `WithHeaders(IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)` | Add multiple headers with multiple values each. |
| `WithAuthentication(string scheme, string token)`                             | Set `Authorization` with a custom scheme.       |
| `WithBasicAuthentication(string token)`                                       | Set `Authorization: Basic {token}`.             |
| `WithBasicAuthentication(string username, string password)`                   | Build and set a Basic auth token.               |
| `WithOAuthBearerToken(string token)`                                          | Set `Authorization: Bearer {token}`.            |
