---
sidebar_position: 5
title: Configure Cookies
---

FluentHttpClient includes simple, focused extensions for attaching cookies to an `HttpRequestBuilder`. Cookies are stored internally on the builder and applied when the final `HttpRequestMessage` is constructed.

## Add a Single Cookie

Use `WithCookie` when you want to attach exactly one cookie to the request.

```csharp
var builder = client
    .UsingBase()
    .WithCookie("session", "abc123");
```

* Throws if `name` is `null`, empty, or whitespace.
* `null` values are converted to an empty string.
* Cookie values are **automatically URL-encoded** by default using RFC 6265 encoding.
* Existing cookies with the same name are overwritten.

### Controlling Encoding

By default, cookie values are URL-encoded to ensure special characters (such as `;`, `=`, `,`, and whitespace) do not break the Cookie header format. This is recommended for most use cases.

```csharp
// Default behavior - value is URL-encoded
var builder = client
    .UsingBase()
    .WithCookie("session", "value with spaces");

// Explicit encoding control
var builder = client
    .UsingBase()
    .WithCookie("session", "value with spaces", encode: true);

// Disable encoding (use with caution)
var preEncodedValue = Uri.EscapeDataString("my value");
var builder = client
    .UsingBase()
    .WithCookie("session", preEncodedValue, encode: false);
```

:::caution When to disable encoding

Set `encode` to `false` only if:
* The value is already properly encoded
* You need to preserve exact byte sequences for legacy systems
* You are certain the value contains no special characters

Disabling encoding with raw special characters can produce malformed Cookie headers.

:::

## Add Multiple Cookies

Use `WithCookies` to attach multiple cookies in one call.

```csharp
var cookies = new[]
{
    new KeyValuePair<string, string>("session", "abc123"),
    new KeyValuePair<string, string>("theme", "dark"),
};

var builder = client
    .UsingBase()
    .WithCookies(cookies);
```

* Throws if the collection itself is `null`.
* Throws if any cookie name is `null`, empty, or whitespace.
* Adds or overwrites existing entries.
* `null` values are stored as empty strings.
* Cookie values are **automatically URL-encoded** by default.

### Controlling Encoding for Multiple Cookies

The `encode` parameter applies to all cookies in the collection:

```csharp
// Default behavior - all values are URL-encoded
var builder = client
    .UsingBase()
    .WithCookies(cookies);

// Explicit encoding control
var builder = client
    .UsingBase()
    .WithCookies(cookies, encode: true);

// Disable encoding for all cookies
var builder = client
    .UsingBase()
    .WithCookies(preEncodedCookies, encode: false);
```

## Behavior Notes

* Cookies are accumulated in `HttpRequestBuilder.Cookies`.
* Assigning the same cookie name overwrites the previous value.
* Cookies are applied to the final request using a standard `Cookie` header.
* Whitespace-only names are treated as invalid.
* **Values are URL-encoded by default** using `Uri.EscapeDataString` ([RFC 6265](https://datatracker.ietf.org/doc/html/rfc6265) compliance).
* When `encode` is `false`, values are stored exactly as provided (aside from `null` becoming an empty string).
* The encoding option defaults to `true` for safety and RFC compliance.
* Multiple cookies are assembled into a single `Cookie` header using semicolon separators.

:::note

Unlike query parameters or headers, cookies do not support multiple values per name.

:::

---

## Quick Reference

| Method                                                        | Purpose                             |
| ------------------------------------------------------------- | ----------------------------------- |
| `WithCookie(string name, string value, bool encode = true)`   | Adds or overwrites a single cookie. |
| `WithCookies(IEnumerable<KeyValuePair<string,string>>, bool encode = true)` | Add multiple cookies at once. |

**Parameters:**
* `name` - The cookie name (required, cannot be null/empty/whitespace)
* `value` - The cookie value (null becomes empty string)
* `encode` - Whether to URL-encode the value (default: `true`)
