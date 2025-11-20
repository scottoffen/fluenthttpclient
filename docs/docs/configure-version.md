---
sidebar_position: 6
title: Configure Version
---

FluentHttpClient provides simple, readable extensions for configuring both the request version and its negotiation behavior.

## Setting HTTP Version

These overloads assign a specific HTTP version to the request. They do not enforce negotiation behavior unless a version policy is also specified.

### Using a Version String

```csharp
builder.UsingVersion("2.0");
```

* Throws if the string is null/empty/whitespace.
* Validates the version with `Version.TryParse`.
* Common examples: `"1.1"`, `"2.0"`, `"3.0"`.

### Using Major/Minor Components

```csharp
builder.UsingVersion(1, 1);
```

Creates a `Version` instance using the numeric components.

### Using `Version` Instance

```csharp
builder.UsingVersion(new Version(2, 0));
```

* Throws if `version` is null.
* Useful when version objects are constructed elsewhere.

## Setting Version and Policy

:::important

Setting the version policy is only supported in NET 5.0+.

:::

The `HttpVersionPolicy` enum controls how `HttpRequestMessage.Version` is interpreted:

* `RequestVersionExact`
* `RequestVersionOrLower`
* `RequestVersionOrHigher`

Combined with explicit version selection, these extensions let you control fallback behavior and protocol negotiation.

### Version String and Policy

```csharp
builder.UsingVersion("2.0", HttpVersionPolicy.RequestVersionExact);
```

### Version Instance Policy

```csharp
builder.UsingVersion(new Version(3, 0), HttpVersionPolicy.RequestVersionOrLower);
```

Both overloads:

* Set `builder.Version`.
* Set `builder.VersionPolicy`.
* Throw if `version` is null (for the `Version` overload).

## Setting Version Policy

:::important

Setting the version policy is only supported in NET 5.0+.

:::

```csharp
builder.UsingVersionPolicy(HttpVersionPolicy.RequestVersionOrHigher);
```

* Leaves the previously assigned version intact.
* Applies only the policy.

## Behavior Notes

Most applications never need to manually set the HTTP version - `HttpClient` negotiates the best protocol the server and handler support. However, there are still practical scenarios where explicitly specifying the version or version policy is useful.

You might override the HTTP version to:
- Force HTTP/1.1 for servers, proxies, or middleware that do not behave correctly with HTTP/2.
- Opt into HTTP/2 where multiplexing, header compression, or improved performance is available and required.
- Test behavior across protocol versions during diagnostics or compatibility checks.
- Control fallback behavior using `HttpVersionPolicy` (e.g., require an exact match or permit downgrade).

---

## Quick Reference

| Method                                     | Purpose                                                |
| ------------------------------------------ | ------------------------------------------------------ |
| `UsingVersion(string)`                     | Set version using a string such as `"1.1"` or `"2.0"`. |
| `UsingVersion(int, int)`                   | Set version using major/minor components.              |
| `UsingVersion(Version)`                    | Set version from a `Version` instance.                 |
| `UsingVersion(string, HttpVersionPolicy)`  | Set version and policy (NET 5.0+).                     |
| `UsingVersion(Version, HttpVersionPolicy)` | Set version and policy (NET 5.0+).                     |
| `UsingVersionPolicy(HttpVersionPolicy)`    | Set negotiation policy only (NET 5.0+).                |
