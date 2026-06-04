---
sidebar_position: 16
title: Custom Builders
---

A custom builder is a subclass of `HttpRequestBuilder` that you have shaped for a specific job. It is still a builder in every way: it carries the full fluent API, it is scoped to a single request, and you get one from an extension method on `HttpClient`, exactly as you get the base builder. Subclassing just lets you add your own methods and change behavior in one place.

## Why Subclass

You keep the full fluent API. Your builder is an `HttpRequestBuilder`, so `WithHeader`, `WithQueryParameter`, `GetAsync`, the response handlers, and the deserialization extensions all work on it with no forwarding code.

You can add your own methods, and they chain. The fluent extensions return your subtype rather than the base type, so a helper you add stays reachable in the middle of a chain, next to the built-in methods.

The class has one behavior. Every other member is a property, a behavior flag, or a method that ends up calling `SendAsync`. Override it and your logic runs on every request the builder sends, whether that request comes from `GetAsync`, a string-based overload, or anything else.

## A Custom Builder

Here is a small builder for the GitHub API. It adds a fluent helper and overrides `SendAsync` to apply a header to every request.

```csharp
using FluentHttpClient;

public class GitHubBuilder : HttpRequestBuilder
{
    public GitHubBuilder(HttpClient client) : base(client)
    {
    }

    public GitHubBuilder(HttpClient client, string route) : base(client, route)
    {
    }

    public GitHubBuilder(HttpClient client, Uri uri) : base(client, uri)
    {
    }

    // Added functionality: a fluent helper that returns the subtype, so it keeps chaining.
    public GitHubBuilder WithApiVersion(string version)
    {
        InternalHeaders["X-GitHub-Api-Version"] = new[] { version };
        return this;
    }

    // Overridden behavior: runs on every request this builder sends.
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

The constructors forward to the base. The base constructors are `protected internal`, which is what lets you chain to them with `base(...)` from your own assembly. This builder declares all three base signatures, which matters for the typed extensions below.

## Obtaining an Instance

You obtain a custom builder the same way you obtain the base builder, from an extension on `HttpClient`. There are two ways to do it.

### A Typed Extension

The library ships generic versions of `UsingBase` and `UsingRoute` that take the builder type:

```csharp
var gh = client.UsingRoute<GitHubBuilder>("/users/octocat");
```

These construct the builder for you, by matching a constructor to the arguments through reflection. A subclass you intend to use with them must declare constructors that mirror the base constructors, so if you rely on the generic extensions, declare all three, `(HttpClient)`, `(HttpClient, string)`, and `(HttpClient, Uri)`, as the sample above does. A call whose signature your subclass does not mirror fails at runtime.

Building through reflection is convenient but not trimming or Native AOT safe, so these overloads carry the matching warnings. Use them when you are not trimming.

### Your Own Extension

For a trimming and AOT safe path, and for the cleanest call site, write a small extension that returns your builder:

```csharp
public static class GitHubBuilderExtensions
{
    public static GitHubBuilder UsingGitHub(this HttpClient client, string route) => new(client, route);
}
```

```csharp
var gh = client.UsingGitHub("/users/octocat");
```

This calls the constructor directly, so there is no reflection, no warnings, and it works on every target. It also reads well, since the name can say what the builder is for.

Either way, your own methods and the built-in ones chain together, and the type is preserved the whole way:

```csharp
var user = await client
    .UsingGitHub("/users/octocat")
    .WithApiVersion("2022-11-28")
    .WithHeader("X-Trace-Id", traceId)
    .GetAsync()
    .ReadJsonAsync<User>();
```

## Lifetime

A custom builder is a per-request object, the same as the base builder. It is the fluent front for a single `HttpRequestMessage`: it accumulates the state of one request and is meant to be used once and let go. Get one from the `HttpClient` for each request, send it, and move on. There is nothing to hold onto or reuse.