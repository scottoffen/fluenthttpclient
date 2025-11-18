---
sidebar_position: 6
title: Response Handling
---

FluentHttpClient includes a set of helper extension methods for working with `HttpResponseMessage` instances. These methods make it easy to read response data, handle success or failure, and perform common actions without repetitive boilerplate.

## Reading Response Content

Most APIs return data in the response body. FluentHttpClient provides convenience methods to read this data in different formats.

### `GetResponseStringAsync()`

Reads the response body as a string.

```csharp
var response = await client.UsingRoute("/users/1").GetAsync();
var content = await response.GetResponseStringAsync();

Console.WriteLine(content);
```

If the response has no content, an empty string is returned.

You can also call this directly on a task:

```csharp
var content = await client
    .UsingRoute("/users/1")
    .GetAsync()
    .GetResponseStringAsync();
```

### `GetResponseBytesAsync()`

Reads the response body as a byte array.

```csharp
var bytes = await client.UsingRoute("/images/logo.png").GetAsync().GetResponseBytesAsync();
await File.WriteAllBytesAsync("logo.png", bytes);
```

This is useful for file downloads or working with binary responses.

### `GetResponseStreamAsync()`

Reads the response body as a stream.

```csharp
await using var stream = await client.UsingRoute("/videos/sample.mp4").GetAsync().GetResponseStreamAsync();
using var file = File.Create("sample.mp4");
await stream.CopyToAsync(file);
```

Streaming is ideal for large downloads or scenarios where you want to process data as it arrives.

## Handling Success and Failure

These methods make it simple to define behavior for successful or failed HTTP responses without complex conditionals.

:::danger Avoid Reading The Response Stream

Avoid reading or consuming the response content (e.g., `GetResponseStringAsync()`, `GetResponseBytesAsync()`, etc.) inside these handlers unless you are certain the response will not be processed elsewhere. Once the response body is read, the underlying stream is consumed, which means later attempts to access the content will return empty results or throw exceptions.

If you need to log or inspect the response body, consider buffering it or cloning the response before reading:

```csharp
.OnSuccessAsync(async response =>
{
    var buffer = await response.GetResponseBytesAsync();

    // Inspect the response here, e.g.:
    var text = Encoding.UTF8.GetString(buffer);

    // Reassign the buffered content if it will be used later
    response.Content = new ByteArrayContent(buffer);
})
```

:::

### `OnSuccess()` and `OnSuccessAsync()`

Execute an action or async function when the response indicates success (`IsSuccessStatusCode == true`).

```csharp
await client.UsingRoute("/users")
    .GetAsync()
    .OnSuccess(response => Console.WriteLine($"Success: {response.StatusCode}"));
```

Async example:

```csharp
await client.UsingRoute("/users")
    .GetAsync()
    .OnSuccessAsync(async response =>
    {
        await _someService.MakeAsyncCall();
        Console.WriteLine($"Success: {response.StatusCode}");
    });
```

### `OnFailure()` and `OnFailureAsync()`

Execute an action or async function when the response indicates failure (`IsSuccessStatusCode == false`).

```csharp
await client.UsingRoute("/users/9999")
    .GetAsync()
    .OnFailure(response => Console.WriteLine($"Failed: {response.StatusCode}"));
```

Async example:

```csharp
await client.UsingRoute("/users/9999")
    .GetAsync()
    .OnFailureAsync(async response =>
    {
        await _someService.MakeAsyncCall();
        Console.WriteLine($"Failed: {response.StatusCode}"));
    });
```

These methods are chainable and non-blocking - they return the original `HttpResponseMessage` so you can continue processing.

## Combining Response Actions

You can chain success and failure handlers together for clean, expressive workflows:

```csharp
var userId = 42;
await client.UsingRoute($"/users/{userId}")
    .GetAsync()
    .OnSuccessAsync(async res =>
    {
        Console.WriteLine($"Fetched user: {userId}");
    })
    .OnFailureAsync(async res =>
    {
        Console.WriteLine($"Error fetching user {userId} : {res.StatusCode}");
    });
```

This allows you to separate happy-path and error-handling logic clearly.

## Summary

FluentHttpClient's response extensions simplify reading and reacting to HTTP responses. Whether you need to quickly read text, stream binary data, or trigger behavior based on success or failure, these methods provide an expressive and consistent way to handle responses in your applications.
