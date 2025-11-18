---
sidebar_position: 3
title: Create Request Builder
---

# Creating Request Builders

To start using **FluentHttpClient**, create a request with the `HttpRequestBuilder` constructor or one of the provided `HttpClient` extension methods.

These methods define the route and prepare a new request builder for further configuration.

## Creating a Request

The easiest way to start a new request is by using the fluent API:

### Create Request From Route

```csharp
var request = client.UsingRoute("/users/12345");
```

This method creates an `HttpRequestBuilder` and sets the `Route` property.

Alternatively, you can create a builder directly:

```csharp
var request = new HttpRequestBuilder(client, "/users/12345");
```

Both approaches are equivalent — the fluent API simply provides a more concise and expressive syntax.

### Create Request Without Route

If your `HttpClient` instance already has a `BaseUrl` configured and no route is needed, you can start a request without specifying one:

```csharp
var request = client.WithoutRoute();
```

This is equivalent to:

```csharp
var request = new HttpRequestBuilder(client);
```

## Route Property

The `Route` property can be changed at anytime before sending the request. However, this property isn't exposed via a fluent method.