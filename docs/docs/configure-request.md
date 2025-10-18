---
sidebar_position: 4
title: Configure Requests
---

# Configuring Requests

FluentHttpClient provides a rich set of extension methods for configuring your `HttpRequestBuilder` instances. These methods let you express intent clearly - whether you're setting headers, adding authentication, defining query strings, or preparing content - while keeping your code concise and consistent.

All the code samples below will use a `request` variable instantiated like this:

```csharp
var request = client.UsingRoute("/some/endpoint");
```

All of the extension methods listed here return the instance of `HttpRequestBuilder`, and are therefore fluent and chainable.

## Authentication

Authentication headers can be configured fluently using the following methods:

### `WithAuthentication(scheme, token)`

Sets a custom authentication scheme and token.

```csharp
request.WithAuthentication("CustomSchema", customToken);
```

### `WithOAuthBearerToken(token)`

Shortcut for setting the `Authorization` header to `Bearer`.

```csharp
request.WithOAuthBearerToken(token);
```

### `WithBasicAuthentication(username, password)`

Adds a Basic authentication header using a username and password. The credentials will automatically be Base64-encoded.

```csharp
request.WithBasicAuthentication("username", "password");
```

This is the most common approach for basic HTTP authentication and ensures proper credential encoding.

### `WithBasicAuthentication(token)`

Adds a Basic authentication header using a pre-encoded token.

```csharp
request.WithBasicAuthentication(encodedToken);
```

Use this when you already have an encoded Basic token and don't need the username/password encoding handled for you.

## Headers

Headers are used to include metadata and control information with each request.

### `WithHeader(key, value)`

Adds a single header.

```csharp
request.WithHeader("X-Request-ID", Guid.NewGuid().ToString());
```

### `WithHeaders(headers)`

Adds multiple headers from a collection of key-value pairs.

```csharp
var headers = new[]
{
    new KeyValuePair<string, string>("X-Source", "ClientA"),
    new KeyValuePair<string, string>("X-Trace", "Enabled")
};

request.WithHeaders(headers);
```

Header actions are applied at send time, ensuring they integrate seamlessly with other configuration methods.

## Query Strings

Query parameters help refine or filter the results of your requests.

### `WithQueryParam(key, value)`

Adds a single query parameter.

```csharp
request.WithQueryParam("active", true);
```

### `WithQueryParams(values)`

Adds multiple parameters from a collection or `NameValueCollection`.

```csharp
var parameters = new Dictionary<string, string?>
{
    ["limit"] = "10",
    ["offset"] = "20"
};

request.WithQueryParams(parameters);
```

### `WithQueryParamIfNotNull(key, value)`

Adds a parameter only if the value is non-null, helping avoid unnecessary empty keys in your query string.
Do not use this method if the API you are calling is expecting the key to be send **even if the value is null**.

```csharp
string? value = null;

request.WithQueryParamIfNotNull("filter", value);
// filter is not added because the value is null
```

## Content

FluentHttpClient supports multiple content formats for outgoing requests.

### `WithContent(content)`

Sets the request body directly using a string or [`HttpContent`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpcontent).

```csharp
var value = "Hello, World!";
var content = new StringContent(value, "text/plain");

// These two statements produce the same result
request.WithContent(value);
request.WithContent(content);
```

### `WithJsonContent(content)`

Serializes an object to JSON using either default or custom `JsonSerializerOptions`.

```csharp
var user = new { Name = "Alice", Email = "alice@example.com" };
request.WithJsonContent(user); // uses default json serializer options
```

### `WithXmlContent(content)`

Serializes an object or raw XML string using the built-in `FluentXmlSerializer`.

```csharp
var order = new Order { Id = 1001, Amount = 49.99M };
request.WithXmlContent(order);
```

:::important XML Serialization

If the object serialization does not serialize to your satisfaction, serialize the object yourself, then pass the raw XML as a string instead of the object.

:::

## Cookies

Cookies can be added directly to the `HttpRequestBuilder`'s cookie container.

### `WithCookie(cookie)`

Adds a cookies to the request.

```csharp
var cookie = new Cookie("Session", "xyz123", "/", "example.com");
request.WithCookie(cookie);
```

### `WithCookie(cookieCollection)`

Adds multiple cookies to the request.

```csharp
var cookies = new CookieCollection
{
    new Cookie("SessionId", "abc123", "/", "example.com"),
    new Cookie("Theme", "dark", "/", "example.com")
};

request.WithCookie(cookies);
```

## Options and Versioning

### `ConfigureOptions(action)`

Configures the request's [`HttpRequestOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestoptions) for advanced scenarios such as retry counts or diagnostic metadata.

```csharp
request.ConfigureOptions(options =>
    {
        options.Set("RetryCount", 3);
        options.Set("RequestID", Guid.NewGuid());
    });
```

### `UsingVersion(version)` and `UsingVersion(version, policy)`

Defines the HTTP protocol version and version negotiation policy.

```csharp
request.UsingVersion("2.0", HttpVersionPolicy.RequestVersionOrLower);
```

This ensures requests use the desired HTTP version while maintaining compatibility with the target server.

## Preloading Content

### `WithPreloadedContent()`

Buffers the request content before sending. This can be useful when working with proxies or when the server requires a `Content-Length` header.

```csharp
request.WithPreloadedContent();
```

> **Note:** Buffering the request into memory will happen during pre-flight, so it's perfectly fine to call this anytime before sending the request.

## Summary

Each of these configuration methods contributes to a composable, fluent workflow. You can combine them freely to express intent clearly, for example:

```csharp
var response = await client
    .UsingRoute("/api/users")
    .WithOAuthBearerToken(token)
    .WithQueryParam("sort", "name")
    .WithJsonContent(newUser)
    .WithHeader("X-Client", "FluentHttpClient")
    .SendAsync(HttpMethod.Post);
```

These methods allow your requests to be readable, predictable, composable, and easy to extend.
