---
sidebar_position: 16
title: Purpose-Built Clients
---

A purpose-built client is a thin class of your own that derives from `HttpRequestBuilder`. Because it is a builder, every FluentHttpClient extension already works on it, so callers get the same fluent experience they would from any builder. What you add on top is a single place to change behavior or expose your own methods.

This page covers the inheritance approach. The more common approach, a plain class that holds an `HttpClient` and forwards to it, is the standard wrapper pattern and is covered elsewhere in these docs. Reach for inheritance when you want the builder's own fluent surface plus a place to override or extend it.

## Why Subclass HttpRequestBuilder

Subclassing gives you three things in one small class.

You keep the full fluent API. Your client is an `HttpRequestBuilder`, so `WithHeader`, `WithQueryParameter`, `GetAsync`, the response handlers, and the deserialization extensions all work on it without any forwarding code.

You get one place to change behavior. Override `SendAsync` and your logic runs on every request the client sends, which is a natural home for an auth header, a correlation id, or logging.

You can add your own methods, and they chain. The fluent extensions return your subtype, not the base `HttpRequestBuilder`, so a helper you add stays reachable in the middle of a chain alongside the built-in methods.

## Service Lifetime

`HttpRequestBuilder` is the fluent front for a single `HttpRequestMessage`. It accumulates the state of one request, the route, headers, query string, and content, and is meant to be used once and thrown away. A subclass inherits that nature, so a purpose-built client is a per-request object.

:::important Per request, not per application
A purpose-built client is short-lived, like the `HttpRequestMessage` it builds, not long-lived like `HttpClient`. Create one, send one request, discard it. Do not cache an instance and reuse it across requests, because its accumulated state would carry over.
:::

The `HttpClient` underneath stays long-lived. That is the point of the factory approach below: the client is created fresh per request, while the `HttpClient` it wraps comes from `IHttpClientFactory`, which pools and rotates the expensive handler. You end up with two layered lifetimes, a long-lived transport and a short-lived request builder on top of it.

## A Purpose-Built Client

Here is a small client for the GitHub API. It exposes route selection, adds one fluent helper, and overrides `SendAsync` to apply a header to every request.

```csharp
using FluentHttpClient;

public class GitHubClient : HttpRequestBuilder
{
    public GitHubClient(HttpClient client) : base(client)
    {
    }

    // Route selection exposed on the client, the way HttpClient.UsingRoute works.
    public GitHubClient UsingRoute(string route)
    {
        SetRoute(route);
        return this;
    }

    // Added functionality: a fluent helper that returns the subtype, so it keeps chaining.
    public GitHubClient WithApiVersion(string version)
    {
        InternalHeaders["X-GitHub-Api-Version"] = new[] { version };
        return this;
    }

    // Overridden behavior: runs on every request this client sends.
    public override Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        HttpCompletionOption completionOption,
        CancellationToken cancellationToken)
    {
        InternalHeaders["Accept"] = new[] { "application/vnd.github+json" };
        return base.SendAsync(method, completionOption, cancellationToken);
    }
}
```

A few things worth calling out.

The constructor forwards to the base. The base constructors are `protected internal`, which is what lets you chain to them with `base(...)` from your own assembly. `UsingRoute` sets the route through the protected `SetRoute`, which a subclass can call to set or change the route after construction. `SetRoute` is `protected`, so it stays out of the standard fluent surface; your client decides whether to expose route selection at all, and how. Because the route no longer has to be fixed at construction, the factory below can hand you a client without knowing the route up front.

You override one method, not many. `SendAsync(HttpMethod, HttpCompletionOption, CancellationToken)` is the only virtual overload. Every other send method, including `GetAsync` and the string-based overloads, routes through it, so this one override covers them all.

`UsingRoute` and `WithApiVersion` both return `GitHubClient`. Because the built-in extensions also return your subtype, you can mix your own methods into a chain and the type is preserved the whole way:

```csharp
var user = await client
    .UsingRoute("/users/octocat")
    .WithApiVersion("2022-11-28")
    .WithHeader("X-Trace-Id", traceId)
    .GetAsync()
    .ReadJsonAsync<User>();
```

## Creating Instances

There are two ways to hand these out through dependency injection. They differ in lifetime and wiring. In both, the route is chosen on the client with `UsingRoute`.

### With a Factory (recommended)

A factory is the better fit for a per-request client. The factory itself is long-lived, and each call returns a fresh client. The factory does not need to know the route. The caller chooses it on the client with `UsingRoute`, so `Create` takes nothing.

```csharp
public class GitHubClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;

    public GitHubClientFactory(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public GitHubClient Create() => new(_httpClientFactory.CreateClient("github"));
}
```

Register the named `HttpClient` for its base address and shared configuration, then register the factory as a singleton.

```csharp
services.AddHttpClient("github", client =>
{
    client.BaseAddress = new Uri("https://api.github.com/");
    client.DefaultRequestHeaders.UserAgent.ParseAdd("my-app");
});

services.AddSingleton<GitHubClientFactory>();
```

Inject the factory and create a client per request.

```csharp
public class ProfileService
{
    private readonly GitHubClientFactory _clients;

    public ProfileService(GitHubClientFactory clients) => _clients = clients;

    public Task<User?> GetUserAsync(string login) =>
        _clients
            .Create()
            .UsingRoute($"/users/{login}")
            .WithApiVersion("2022-11-28")
            .GetAsync()
            .ReadJsonAsync<User>();
}
```

The factory is a singleton and safe to reuse. Each `GitHubClient` it returns is used for one request and discarded. `IHttpClientFactory` keeps the underlying handler pooled, so creating a client per request stays cheap.

### As a Typed Client

You can also register the client directly as a typed client. The container constructs it and injects the configured `HttpClient`.

```csharp
services.AddHttpClient<GitHubClient>(client =>
{
    client.BaseAddress = new Uri("https://api.github.com/");
    client.DefaultRequestHeaders.UserAgent.ParseAdd("my-app");
});
```

A typed client is registered as transient and constructed from the configured `HttpClient`. You choose the route on it with `UsingRoute`, the same as with the factory. Inject it where you need it.

```csharp
public class StatusService
{
    private readonly GitHubClient _client;

    public StatusService(GitHubClient client) => _client = client;

    public Task<HttpResponseMessage> PingAsync() =>
        _client.UsingRoute("/zen").GetAsync();
}
```

:::important Resolve one per request

Because the typed client is still a per-request builder, treat each resolved instance as single use. It is transient, so resolve a fresh one for each request rather than holding it in a longer-lived service and reusing it.

:::

## Choosing Between Them

Use the factory in most cases. It fits the per-request nature of the builder, the long-lived factory produces a fresh client for each request, and the per-request lifetime is obvious at the call site.

Reach for a typed client when you want the least wiring and the consuming service is already scoped to a single request, so a freshly resolved instance is used once and discarded. Outside that, the factory is the safer default.