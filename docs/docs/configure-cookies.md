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
* Existing cookies with the same name are overwritten.

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

:::note

Unlike query parameters or headers, cookies do not support multiple values per name.

:::

## Behavior Notes

* Cookies are accumulated in `HttpRequestBuilder.Cookies`.
* Assigning the same cookie name overwrites the previous value.
* Cookies are applied to the final request using a standard `Cookie` header.
* Whitespace-only names are treated as invalid.
* Values are stored exactly as provided (aside from null becoming an empty string).

---

## Quick Reference

| Method                                                  | Purpose                             |
| ------------------------------------------------------- | ----------------------------------- |
| `WithCookie(string, string)`                            | Adds or overwrites a single cookie. |
| `WithCookies(IEnumerable<KeyValuePair<string,string>>)` | Add multiple cookies at once.       |
