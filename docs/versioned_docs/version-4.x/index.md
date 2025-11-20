---
sidebar_position: 1
title: Introduction
---

# FluentHttpClient

FluentHttpClient exposes a set of extensions methods to make sending REST requests with `HttpClient` both readable and chainable.

## Using HttpClient

[HttpClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) is the [recommended way](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines) to make HTTP requests in C# applications for several reasons:

- **Asynchronous Operations**: `HttpClient` is designed for asynchronous programming with async/await, which is crucial for building responsive applications that don't block the main thread while waiting for network responses.

- **Connection Pooling**: It efficiently manages and reuses HTTP connections, reducing overhead, especially for secure HTTPS connections where the handshake is performed only once. This optimizes performance by minimizing the creation of new connections.

- **Extensibility and Customization**: `HttpClient` utilizes a plug-in architecture based on [HttpMessageHandler](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpmessagehandler). This allows developers to customize the underlying HTTP behavior, such as implementing custom caching, handling cookies, or even replacing the entire network stack with a native implementation on platforms like iOS and Android.

- **Resilience and Policies**: When used with [`IHttpClientFactory`](https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory) in ASP.NET Core, `HttpClient` can be integrated with policies for handling transient faults, such as retries with exponential backoff and circuit breakers, improving the robustness of network communication in distributed systems (e.g. using [Polly](https://www.pollydocs.org/))

- **Centralized Configuration**: `IHttpClientFactory` provides a central location for configuring `HttpClient` instances, including setting default headers, base addresses, and applying middleware, which simplifies the management of HTTP clients, especially for different external APIs.

- **Lifetime Management and Socket Exhaustion**: `IHttpClientFactory` manages the pooling and lifetime of `HttpMessageHandler` instances, preventing common issues like socket exhaustion that can occur with improper manual `HttpClient` lifetime management.

- **`CancellationToken` Support**: `HttpClient` supports `CancellationToken` for canceling long-running HTTP requests, which is essential for user experience and resource management in applications.

:::note

The socket exhaustion problems associated with the incorrect usage of the HttpClient class in .NET applications has been well documented. Microsoft has published [an article introducing `IHttpClientFactory`](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0), which is used to configure and create HttpClient instances in an app.

:::

## Using FluentHttpClient

Using `HttpClient` involves creating an [`HttpRequestMessage`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestmessage), configuring it's properties (e.g. headers, query string, route, etc.), serializing any content that should be in the body of the request, and finally sending the request and getting a response. The response then needs to be inspected and deserialized.

The extension methods available in this library are intended to simplify that lifecycle. These extension methods fall into three categories:

### Configuring the `HttpRequestMessage`

The `UsingRoute()` extension method on `HttpClient` returns an `HttpRequestBuilder` object, which exposes extension methods on it for configuring the request.

### Sending the Request

`HttpRequestBuilder` also has extension methods for sending the request using the common HTTP verbs, and a generic method for using custom verbs.

### Deserializing the Response

Finally, there are extension methods for both [`HttpResponseMessage`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage) and `Task<HttpResponseMessage>` for deserializing the content returned in the response.


### Example

To enjoy the benefits of using these chaining methods, you can configure the request and send it all in one chain. The example below proceeds in that topical order.

```csharp
var content = await _client
    .UsingRoute("/repos/scottoffen/grapevine/issues")
    .WithQueryParam("state", "open")
    .WithQueryParam("sort", "created")
    .WithQueryParam("direction", "desc")
    .GetAsync()
    .GetResponseStreamAsync();
```

:::important

Note that the method name difference between setting a single instance of a property and multiples is that the multiple instance will use the plural form. For example, you can add a single query parameter using the method `WithQueryParam()` and multiple query parameters at once using `WithQueryParams()`.

:::