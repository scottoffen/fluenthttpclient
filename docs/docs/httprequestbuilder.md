---
sidebar_position: 1
title: HttpRequestBuilder
---

`HttpRequestBuilder` is the core building block of FluentHttpClient. It wraps an existing `HttpClient` instance and gives you a focused, chainable way to build up a single HTTP request before sending it.

Instead of configuring headers, cookies, query strings, content, and HTTP version directly on `HttpRequestMessage`, you work with a builder that tracks all of that state. Extension methods hang off `HttpRequestBuilder` to keep the call site clean and readable. When you finally call `SendAsync` (or one of it's overloads), the builder composes the URI, applies headers, cookies, options, and content, and then uses the underlying `HttpClient` to send the request.

At a high level, the workflow looks like this:
- You create an `HttpRequestBuilder` from an existing `HttpClient` (optionally with a route).
- You use fluent extension methods to configure the builder state.
- You call `SendAsync` with the desired `HttpMethod` (or one of it's overloads) to send the request.

This keeps each request self-contained and avoids mutating shared `HttpClient` state like `DefaultRequestHeaders`.

## Fluent Workflow

FluentHttpClient is built around a simple pattern:

```csharp
var response = await client
    .UsingRoute("/api/widgets")   // produces a HttpRequestBuilder
    .WithHeader("X-Tenant", tenantId)
    .WithQueryParameter("state", "active")
    .WithJsonContent(payload)
    .PostAsync();
```

In this example:
- The `UsingRoute` extension constructs an `HttpRequestBuilder`.
- Each `With*` call adds configuration to the builder by updating its properties and configurator collections.
- `PostAsync` (an overload for `SendAsync(HttpMethod.Post)`) uses the builder state to:
    - Build the final request URI from `HttpClient.BaseAddress`, `Route`, and `QueryParameters`.
    - Apply headers, cookies, and options.
    - Attach content and set HTTP version and version policy.
    - Optionally buffer the request content if requested (not shown in this example).
    - Send the request via the underlying `HttpClient`.

`HttpRequestBuilder` itself is intentionally small and focused. It holds state and provides the minimal sending API. All the DX sugar lives in extension methods that operate on the builder.

## Constructors

Starting in FluentHttpClient 5.0, `HttpRequestBuilder` is no longer able to be constructed directly. Its constructors are now internal to ensure consistent validation and to guide users toward the supported creation patterns. To begin building a request, use one of the `HttpClient` extension methods described below. Each one returns a fresh builder instance scoped to a single request.

:::warning Query Strings and Fragments

`BaseAddress` and `Route` must remain clean—free of query strings and fragments—so that FluentHttpClient has a single, predictable source of truth for all query-related behavior. Allowing query components in multiple places leads to ambiguous URI construction, duplicated encoding, and inconsistent request signatures. By enforcing that all query values flow through `QueryParameters`, the builder can reliably compose the final URI, ensure consistent encoding rules, and prevent subtle bugs caused by mixing inline query strings with fluent configuration.

:::

### `UsingBase()`

```csharp
var bulder = client.UsingBase();
```

Creates a new `HttpRequestBuilder` using the `HttpClient`'s configured `BaseAddress` as the starting point. This is the preferred way to build requests when your client already has a base URI set - or will before the request is sent -and no additional route is needed.

Use this when:
- You already set `HttpClient.BaseAddress`.
- You want all requests to be sent to that `Uri`.

*If `BaseAddress` is not set prior to sending the request an exception will be thrown.*

### `UsingRoute(string route)`

```csharp
var builder = client.UsingRoute("api/v1/users");
```

Creates a builder using the provided `route` as the initial request URI. The `route` may be either:
- A relative path (e.g. "api/widgets/42"), or
- An absolute URI ("https://api.example.com/v1/widgets").

FluentHttpClient validates that the route does not include query strings or fragments. All query parameters should be added through the builder's `QueryParameters` collection (preferably through the provided overloads).

Use this overload when you want to target an endpoint that is either different from or relative to the `BaseAddress` property on the client.

### `UsingRoute(Uri uri)`

```csharp
var builder = client.UsingRoute(new Uri("api/v1/users"));
```

This overload behaves the same as the string version but takes a pre-constructed Uri instance. Use it when:
- You already have a Uri from configuration or another component.
- You want to avoid the overhead or ambiguity of parsing a raw string.
- You need to pass a `UriKind.Absolute` or `UriKind.Relative` explicitly.

As with the string-based overload, the provided `Uri` must not contain query or fragment components.

## Properties

The table below lists the key properties on `HttpRequestBuilder` and how they are used.

| Property                                                 | Description                                                                                                                                                              |
| -------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `HttpContent? Content`                                   | The request body to send. Set by content-related extensions such as JSON, XML, or form encoded data.                                                                          |
| `IDictionary<string,string> Cookies`                     | Per-request cookies serialized into a single `Cookie` header. Values are not encoded automatically; long-lived cookie storage should come from the `HttpClient` handler. |
| `List<Action<HttpRequestBuilder>> DeferredConfigurators` | Actions executed immediately before building the request, enabling late-bound or conditional configuration (used by `.When(...)` extensions).                               |
| `List<Action<HttpRequestHeaders>> HeaderConfigurators`   | Actions that mutate `HttpRequestMessage.Headers` during request construction. Populated by header-related extensions.                                                    |
| `List<Action<HttpRequestOptions>> OptionConfigurators`*  | Actions that set values in `HttpRequestMessage.Options`. Useful for per-request flags, tracing, or contextual data.                                                      |
| `bool BufferRequestContent`                              | Forces the request content to be fully buffered in memory before sending. Intended only for compatibility edge cases where buffering is required.                        |
| `HttpQueryParameterCollection QueryParameters`           | Represents all query string values for the request. The route and base address must not contain query components; this collection is the single source of truth.         |
| `string? Route`                                          | The relative or absolute request route originally provided to the builder; validated so it contains no query or fragment.                                                |
| `Version Version`                                        | The HTTP protocol version applied to the outgoing request. Defaults to HTTP/1.1.                                                                                         |
| `HttpVersionPolicy VersionPolicy`*                       | Controls how the requested HTTP version is interpreted and negotiated (e.g., upgrade, downgrade, or strict). Defaults to `RequestVersionOrLower`.                        |

\* *Available only on target frameworks that support `HttpRequestOptions` / `HttpVersionPolicy` (e.g., .NET 5+).*

## Sending Requests

`HttpRequestBuilder` exposes a small set of sending methods:
- `Task<HttpResponseMessage> SendAsync(HttpMethod method)`
- `Task<HttpResponseMessage> SendAsync(HttpMethod method, CancellationToken cancellationToken)`
- `Task<HttpResponseMessage> SendAsync(HttpMethod method, HttpCompletionOption completionOption)`
- `Task<HttpResponseMessage> SendAsync(HttpMethod method, HttpCompletionOption completionOption, CancellationToken cancellationToken)`

:::important Use Recommended Overloads

While `SendAsync` is the core sending primitive, most consumers should prefer the convenience extensions such as `GetAsync`, `PostAsync`, `PutAsync`, `DeleteAsync`, `HeadAsync`, `OptionsAsync`, and `PatchAsync` (where available). These extensions select the correct `HttpMethod`, keep your call sites clean, and make intent immediately obvious. Use `SendAsync` directly only when you are using a non-standard `HttpMethod`.

:::

All overloads delegate to the most complete overload. That method:

1. Runs `DeferredConfigurators` so late-bound configuration can modify the builder.
2. Optionally buffers Content if `BufferRequestContent` is true.
3. Builds the final URI by combining:
    - `HttpClient.BaseAddress`
    - `Route`
    - `QueryParameters`
4. Creates an `HttpRequestMessage` with:
    - The chosen `HttpMethod`
    - The constructed `Uri`
    - The configured `Content`
    - `Version` and (when available) `VersionPolicy`
5. Applies deferred configurations such as:
    - Disable `ExpectContinue` for multipart content.
    - Apply each `HeaderConfigurator`.
    - Serialize Cookies into the Cookie header.
    - Apply each `OptionConfigurator` (when available).
6. Sends the request via `_client.SendAsync`.

:::danger Experimental Method

For testing and advanced scenarios, there is an experimental `BuildRequest` method that constructs and returns the `HttpRequestMessage` without sending it. This is primarily intended for unit tests and internal usage. **Do not depend on this method being available in future versions.**

:::