---
sidebar_position: 8
title: Configure Timeout
---

# Configure Timeout

FluentHttpClient provides a set of extensions for applying **per-request** timeouts. These timeouts define how long the client will wait for the current request to complete before canceling it. Timeouts are implemented through cancellation, do not alter `HttpClient.Timeout`, and have no effect on other requests made with the same `HttpClient` instance.

## Usage

A timeout may be applied using either a number of seconds or a `TimeSpan`. The timeout must be a positive value. When set, the request will be canceled if the configured interval elapses before the response is received.

```csharp
var response = await client
    .UsingBase()
    .WithRoute("/todos/1")
    .WithTimeout(5)                // 5-second timeout
    .GetAsync();
```

```csharp
var response = await client
    .UsingBase()
    .WithRoute("/todos/1")
    .WithTimeout(TimeSpan.FromMilliseconds(750))
    .GetAsync();
```

Timeouts link with any caller-provided `CancellationToken`; whichever triggers first will cancel the request.

## Behavior Notes

* The timeout applies only to the current request and does not affect any other requests sent using the same `HttpClient` instance.
* The timeout value must be a positive duration; zero or negative values are not allowed and will result in an exception.
* The timeout does not modify `HttpClient.Timeout` and is enforced solely through per-request cancellation.
* The timeout works together with any caller-provided `CancellationToken`, and the request will be canceled when either the timeout expires or the token is triggered.

## Quick Reference

| Method                  | Purpose                                           |
| ----------------------- | ------------------------------------------------- |
| `WithTimeout(int)`      | Applies a per-request timeout using seconds.      |
| `WithTimeout(TimeSpan)` | Applies a per-request timeout using a `TimeSpan`. |
