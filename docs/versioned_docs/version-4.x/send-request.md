---
sidebar_position: 5
title: Send Requests
---

# Sending Requests

With a configured `HttpRequestBuilder` instance, sending a request is as simple as calling one of the provided asynchronous extension methods. Each method corresponds to a common HTTP verb and handles the appropriate request setup for you.

Each of the extensions methods below takes an optional `HttpCompletionOptions` flag and/or `CancellationToken`.

## DeleteAsync

Use `DeleteAsync()` to send an HTTP **DELETE** request.

```csharp
var response = await client
    .UsingRoute($"/users/12345")
    .DeleteAsync();
```

This method automatically creates a `HttpRequestMessage` using `HttpMethod.Delete` and sends it using the underlying `HttpClient`.
It returns an `HttpResponseMessage` that you can inspect or deserialize as needed.

## GetAsync

Use `GetAsync()` to send an HTTP **GET** request.

```csharp
var response = await client
    .UsingRoute("/users")
    .WithQueryParam("limit", 10)
    .WithQueryParam("offset", 0)
    .GetAsync();

if (response.IsSuccessStatusCode)
{
    // Handle response
}
```

This method automatically creates a `HttpRequestMessage` using `HttpMethod.Get` and sends it using the underlying `HttpClient`.
It returns an `HttpResponseMessage` that you can inspect or deserialize as needed.

## PostAsync

Use `PostAsync()` to send an HTTP **POST** request with content.

```csharp
var newUser = new { Name = "Alice", Email = "alice@example.com" };

var response = await client
    .UsingRoute("/users")
    .WithJsonContent(newUser)
    .PostAsync();
```

This method automatically creates a `HttpRequestMessage` using `HttpMethod.Post` and sends it using the underlying `HttpClient`.
It returns an `HttpResponseMessage` that you can inspect or deserialize as needed.
Any `HttpContent` configured on the builder (such as JSON, XML, or form data) will be included in the body of the request.

## PutAsync

Use `PutAsync()` to send an HTTP **PUT** request, often used to update an existing resource.

```csharp
var updatedUser = new { Name = "Alice Updated", Email = "alice@example.com" };

var response = await client
    .UsingRoute($"/users/12345")
    .WithJsonContent(updatedUser)
    .PutAsync();
```

This method automatically creates a `HttpRequestMessage` using `HttpMethod.Put` and sends it using the underlying `HttpClient`.
It returns an `HttpResponseMessage` that you can inspect or deserialize as needed.
Any `HttpContent` configured on the builder (such as JSON, XML, or form data) will be included in the body of the request.

## Other Methods

You can also send requests using any custom or less common HTTP method by calling `SendAsync()` directly.

```csharp
var response = await client
    .UsingRoute("/users/12345")
    .SendAsync(HttpMethod.Head);
```

Or with additional options:

```csharp
var response = await client
    .UsingRoute("/health")
    .SendAsync(HttpMethod.Options, HttpCompletionOption.ResponseHeadersRead);
```

The `SendAsync()` method provides full control over the request method, completion behavior, and cancellation token, making it useful for advanced or non-standard HTTP operations.

## Summary

FluentHttpClient's sending methods provide a clean, expressive way to send HTTP requests without repetitive boilerplate.
Each method automatically constructs and sends the appropriate `HttpRequestMessage`, ensuring consistency and readability across your codebase.
