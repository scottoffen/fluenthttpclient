---
sidebar_position: 11
title: Response Handlers
---

FluentHttpClient includes a set of response handler extensions that allow you to take action based on the result of an HTTP request - without breaking the fluent flow. These methods operate **after the response has been received**, offering a clean way to apply side effects, logging, metrics, or conditional logic.

Response handlers are useful when you want to:

* Run logic only for successful responses (`OnSuccess`).
* Run logic only for failed responses (`OnFailure`).
* Execute custom logic based on arbitrary conditions (`When`).
* Attach observability hooks (logging, metrics, tracing) at the point where the response becomes available.
* Avoid wrapping everything in `if` statements after each awaited request.

These handlers do **not** interfere with normal response processing. Each method returns the original `HttpResponseMessage`, allowing additional chained operations.

:::danger

If the handler reads the response content, subsequent chained operations may not be able to read it again.

:::

## Conditional Response Handling

The `When` extension is the foundation of all response handlers. It evaluates a predicate against the completed `HttpResponseMessage` and runs a handler only when the predicate returns true.

Two overloads are provided: one for synchronous handlers, one for asynchronous handlers.

### Synchronous handler

```csharp
var response = await builder.GetAsync()
    .When(r => r.StatusCode == HttpStatusCode.NotModified,
          r => logger.LogInformation("Not modified"));
```

### Asynchronous handler

```csharp
var response = await builder.PostAsync()
    .When(r => r.StatusCode == HttpStatusCode.BadRequest,
          async r => await LogValidationErrorsAsync(r));
```

## Handling Successful Responses

`OnSuccess` is a convenience wrapper around `When` that triggers if `response.IsSuccessStatusCode` is true.

### Synchronous handler

```csharp
var response = await builder.GetAsync()
    .OnSuccess(r => logger.LogInformation($"Success: {r.StatusCode}"));
```

### Asynchronous handler

```csharp
var response = await builder.GetAsync()
    .OnSuccess(async r => await metrics.RecordSuccessAsync(r));
```

## Handling Failed Responses

`OnFailure` is a convenience wrapper around `When` that triggers if `response.IsSuccessStatusCode` is false.

### Synchronous handler

```csharp
var response = await builder.PostAsync()
    .OnFailure(r => logger.LogError($"Request failed: {r.StatusCode}"));
```

### Asynchronous handler

```csharp
await builder.PostAsync()
    .OnFailure(async r => await metrics.RecordFailureAsync(r));
```

## Usage Examples

### Example: log success, capture failures, continue pipeline

```csharp
var response = await builder
    .WithJsonContent(model)
    .PostAsync()
    .OnSuccess(r => logger.LogInformation("Created"))
    .OnFailure(async r => await SaveFailedResponseAsync(r));
```

### Example: fail fast on error after logging

```csharp
var response = await builder.GetAsync()
    .OnFailure(r => logger.LogWarning($"API returned {r.StatusCode}"));
```

### Example: use a custom predicate

```csharp
var response = await builder.GetAsync()
    .When(r => r.StatusCode == HttpStatusCode.TooManyRequests,
          async r => await HandleRateLimitAsync(r));
```

## Behavior Notes

* The predicate is evaluated **after** the HTTP call completes.
* The response is not disposed automatically - you control its lifetime.
* If the handler reads the response content, subsequent chained operations may not be able to read it again.
* The returned `Task` resolves to the **original response**, whether or not the handler ran.

---

## Quick Reference

| Method                        | Trigger condition               | Handler type |
| ----------------------------- | ------------------------------- | ------------ |
| `When(predicate, Action)`     | `predicate(response)` is true   | Synchronous  |
| `When(predicate, Func<Task>)` | `predicate(response)` is true   | Asynchronous |
| `OnSuccess(Action)`           | `response.IsSuccessStatusCode`  | Synchronous  |
| `OnSuccess(Func<Task>)`       | `response.IsSuccessStatusCode`  | Asynchronous |
| `OnFailure(Action)`           | `!response.IsSuccessStatusCode` | Synchronous  |
| `OnFailure(Func<Task>)`       | `!response.IsSuccessStatusCode` | Asynchronous |

These methods make post-response processing concise, expressive, and fully compatible with fluent request construction.
